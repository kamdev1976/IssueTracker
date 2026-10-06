using IssueTracker.Core.Entities;
using IssueTracker.Core.Interfaces;
using IssueTracker.Data;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IssueTracker.Web
{
    /// <summary>
    /// Code-behind for managing soft-deleted/archived issues and restoring records back to active state.
    /// </summary>
    public partial class Archive : Page
    {
        private IUnitOfWork _unitOfWork;

        /// <summary>
        /// Gets or sets the Unit of Work dependency injected via Autofac.
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (_unitOfWork == null)
                {
                    _unitOfWork = new UnitOfWork(new PrimaryIssueEntities());
                }
                return _unitOfWork;
            }
            set => _unitOfWork = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindArchiveGrid();
            }
        }

        /// <summary>
        /// Binds archived issues (IsDeleted == 1) to the GridView with optional search filtering, 
        /// database-level pagination, and AsNoTracking optimization.
        /// </summary>
        private void BindArchiveGrid()
        {
            // Retrieve repository through the safe UnitOfWork property getter
            var issueRepo = UnitOfWork.GetRepository<Issue>();

            string searchKeyword = ViewState["SearchKeyword"] as string ?? string.Empty;
            string searchBy = ViewState["SearchBy"] as string ?? "All";

            // Query archived records only (IsDeleted == 1) with AsNoTracking optimization for performance
            var query = issueRepo.Find(i => i.IsDeleted == 1).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                searchKeyword = searchKeyword.Trim().ToLower();

                switch (searchBy)
                {
                    case "Title":
                        query = query.Where(i => i.Title != null && i.Title.ToLower().Contains(searchKeyword));
                        break;

                    case "Priority":
                        query = query.Where(i => i.Priority != null && i.Priority.ToLower().Contains(searchKeyword));
                        break;

                    case "AssignedTo":
                        query = query.Where(i => i.AssignedTo != null && i.AssignedTo.ToLower().Contains(searchKeyword));
                        break;

                    default:
                        query = query.Where(i => (i.Title != null && i.Title.ToLower().Contains(searchKeyword)) ||
                                                 (i.Priority != null && i.Priority.ToLower().Contains(searchKeyword)) ||
                                                 (i.AssignedTo != null && i.AssignedTo.ToLower().Contains(searchKeyword)));
                        break;
                }
            }

            // Get total record count for GridView custom virtual pagination
            int totalRecords = query.Count();
            gvArchive.VirtualItemCount = totalRecords;

            // Apply database-level pagination using Skip and Take
            int pageSize = gvArchive.PageSize;
            int pageIndex = gvArchive.PageIndex;

            var pagedData = query.OrderByDescending(i => i.IssueID)
                                 .Skip(pageIndex * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            // Bind paged results to the GridView
            gvArchive.DataSource = pagedData;
            gvArchive.DataBind();
        }

        /// <summary>
        /// Handles search execution for archived issues.
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ViewState["SearchKeyword"] = txtSearch.Text;
            ViewState["SearchBy"] = ddlSearchBy.SelectedValue;
            gvArchive.PageIndex = 0;
            BindArchiveGrid();
        }

        /// <summary>
        /// Resets search criteria and rebinds the grid.
        /// </summary>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            ddlSearchBy.SelectedIndex = 0;
            ViewState["SearchKeyword"] = null;
            ViewState["SearchBy"] = null;
            gvArchive.PageIndex = 0;
            BindArchiveGrid();
        }

        /// <summary>
        /// Row command handler responsible for restoring records (`IsDeleted = 0`).
        /// </summary>
        protected void gvArchive_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RestoreIssue")
            {
                int issueId = Convert.ToInt32(e.CommandArgument);
                RestoreIssueToActive(issueId);
            }
        }

        /// <summary>
        /// Flips the IsDeleted soft flag back to 0 and commits the single database transaction.
        /// </summary>
        /// <param name="issueId">The primary key of the issue to restore.</param>
        private void RestoreIssueToActive(int issueId)
        {
            var issueRepo = UnitOfWork.GetRepository<Issue>();
            var archivedIssue = issueRepo.GetById(issueId);

            if (archivedIssue != null)
            {
                archivedIssue.IsDeleted = 0;
                issueRepo.Update(archivedIssue);
                UnitOfWork.Complete();

                lblMessage.Text = $"Issue #{issueId} was successfully restored to active status.";
                lblMessage.Visible = true;

                BindArchiveGrid();
                upArchive.Update();
            }
        }

        /// <summary>
        /// Handles pagination page index changes.
        /// </summary>
        protected void gvArchive_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvArchive.PageIndex = e.NewPageIndex;
            BindArchiveGrid();
        }
    }
}
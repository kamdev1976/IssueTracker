<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Archive.aspx.cs" Inherits="IssueTracker.Web.Archive" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Archived Issues</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .pagination-container td span {
            background-color: #6c757d;
            color: white;
            padding: 6px 12px;
            border-radius: 4px;
            margin: 0 2px;
            font-weight: bold;
        }
        .pagination-container td a {
            color: #6c757d;
            padding: 6px 12px;
            text-decoration: none;
            border: 1px solid #dee2e6;
            border-radius: 4px;
            margin: 0 2px;
        }
        .pagination-container td a:hover {
            background-color: #e9ecef;
        }
    </style>
</head>
<body class="bg-light">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="container my-4">
            <!-- Header & Navigation -->
            <div class="d-flex justify-content-between align-items-center mb-4">
                <h3 class="fw-bold text-secondary mb-0">Archived Issues</h3>
                <a href="Default.aspx" class="btn btn-outline-primary">&larr; Back to Active Issues</a>
            </div>

            <asp:UpdatePanel ID="upArchive" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <!-- Alert Banner for Notifications -->
                    <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success d-block mb-3" Visible="false" />

                    <!-- Search Section -->
                    <div class="card shadow-sm mb-4">
                        <div class="card-body">
                            <h5 class="card-title fw-bold mb-3">Search Archive</h5>
                            <div class="row g-2 align-items-center">
                                <div class="col-md-3">
                                    <asp:DropDownList ID="ddlSearchBy" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="Search All Fields" Value="All" />
                                        <asp:ListItem Text="Title" Value="Title" />
                                        <asp:ListItem Text="Priority" Value="Priority" />
                                        <asp:ListItem Text="Assigned To" Value="AssignedTo" />
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6">
                                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Type keyword to search archive..."
                                        oninput="triggerArchiveSearch(this);">
                                      </asp:TextBox>
                                    <input type="hidden" id="__activeSearchId" value="" />
                                    <input type="hidden" id="__activeCursorPos" value="" />
                                </div>
                                <div class="col-md-3 d-flex gap-2">
                                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-outline-secondary w-100" OnClick="btnSearch_Click" />
                                    <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-outline-dark w-100" OnClick="btnClear_Click" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Archived Records GridView -->
                    <div class="card shadow-sm">
                        <div class="card-header bg-secondary text-white">
                            <h5 class="mb-0 fw-bold">Archived Records</h5>
                        </div>
                        <div class="card-body p-0">
                            <div class="table-responsive">
                                <asp:GridView ID="gvArchive" runat="server" AutoGenerateColumns="False"
                                    CssClass="table table-striped table-hover align-middle mb-0"
                                    DataKeyNames="IssueID"
                                    AllowPaging="True"
                                    AllowCustomPaging="True"
                                    PageSize="5"
                                    OnRowCommand="gvArchive_RowCommand"
                                    OnPageIndexChanging="gvArchive_PageIndexChanging">

                                    <PagerStyle CssClass="pagination-container my-2" HorizontalAlign="Center" />

                                    <Columns>
                                        <asp:BoundField DataField="IssueID" HeaderText="Issue ID" />
                                        <asp:BoundField DataField="Title" HeaderText="Title" />
                                        <asp:BoundField DataField="Description" HeaderText="Description" />
                                        <asp:BoundField DataField="Priority" HeaderText="Priority" />
                                        <asp:BoundField DataField="AssignedTo" HeaderText="Assigned To" />
                                        <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />

                                        <asp:TemplateField HeaderText="Actions">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnRestore" runat="server"
                                                    CommandName="RestoreIssue"
                                                    CommandArgument='<%# Eval("IssueID") %>'
                                                    CssClass="btn btn-sm btn-outline-success"
                                                    OnClientClick="return confirm('Are you sure you want to restore this issue to active state?');">
                                                    Restore
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Live Search Debounce Script -->
       
    </form>
    <script src="Scripts/Issue-tracker-archived.js" type="text/javascript"></script>
     <script type="text/javascript">
       var searchButtonId = '<%= btnSearch.ClientID %>';
</script>
</body>
</html>
/**
 * Issue Tracker - Modal Helpers & AJAX Lifecycle Management
 * Location: Scripts/issue-tracker.js
 */

/**
 * Opens the Bootstrap 5 Add/Edit Modal
 */
function openModal() {
    var modalEl = document.getElementById('addEditModal');
    if (modalEl) {
        var myModal = bootstrap.Modal.getOrCreateInstance(modalEl);
        myModal.show();
    }
}

/**
 * Closes the Bootstrap 5 Add/Edit Modal
 */
function closeModal() {
    var modalEl = document.getElementById('addEditModal');
    if (modalEl) {
        var myModal = bootstrap.Modal.getInstance(modalEl);
        if (myModal) {
            myModal.hide();
        }
    }
}

/**
 * Resets modal fields and opens modal for creating a new issue
 */
function openModalForNew() {
    clearForm();
    openModal();
}

/**
 * Clears form fields inside the modal UI
 */
function clearForm() {
    var hfId = document.querySelector('[id$="hfIssueID"]');
    var txtTitle = document.querySelector('[id$="txtTitle"]');
    var txtDesc = document.querySelector('[id$="txtDescription"]');
    var ddlPriority = document.querySelector('[id$="ddlPriority"]');
    var txtAssigned = document.querySelector('[id$="txtAssignedTo"]');
    var errLabel = document.querySelector('[id$="lblModalError"]');

    if (hfId) hfId.value = '';
    if (txtTitle) txtTitle.value = '';
    if (txtDesc) txtDesc.value = '';
    if (ddlPriority) ddlPriority.selectedIndex = 0;
    if (txtAssigned) txtAssigned.value = '';

    if (errLabel) {
        errLabel.style.display = 'none';
        errLabel.innerText = '';
    }
}

/**
 * Ensures backdrop elements and body scrolling states are properly restored on hidden modal
 */
function cleanupBackdrop() {
    document.querySelectorAll('.modal-backdrop').forEach(function (el) {
        el.remove();
    });
    document.body.classList.remove('modal-open');
    document.body.style.overflow = '';
    document.body.style.paddingRight = '';
}

/**
 * Attaches event listeners for modal cleanup on DOM load and ASP.NET ScriptManager postbacks
 */
function initModalListeners() {
    var modalEl = document.getElementById('addEditModal');
    if (modalEl && !modalEl.dataset.listenerAttached) {
        modalEl.addEventListener('hidden.bs.modal', cleanupBackdrop);
        modalEl.dataset.listenerAttached = 'true';
    }
}

// Attach listener on initial page load
document.addEventListener('DOMContentLoaded', initModalListeners);

/**
 * issue-tracker.js
 * Handles client-side interactivity, live search debouncing, and modal behaviors for the Issue Tracker.
 */
var searchTimer = null;

/**
 * Triggers live search asynchronously with a debounce delay once at least 3 characters are typed.
 * Preserves current cursor position and focus.
 * @param {HTMLInputElement} txtBox - The search text input element.
 */
function triggerLiveSearch(txtBox) {
    // Clear previous timer while the user is actively typing
    if (searchTimer) {
        window.clearTimeout(searchTimer);
    }

    var keyword = txtBox.value ? txtBox.value.trim() : "";

    // If the user clears the search or types less than 3 characters, 
    // you can optionally reset or wait until 3 characters are reached.
    // If you want it to search immediately when cleared (empty), we handle that:
    if (keyword.length > 0 && keyword.length < 3) {
        return; // Do nothing until at least 3 characters are typed
    }

    // Capture current cursor position and element ID before postback
    var cursorPos = txtBox.selectionStart;
    var boxId = txtBox.id;

    // Wait 300ms after typing stops before executing the search click
    searchTimer = window.setTimeout(function () {
        if (typeof searchButtonId !== 'undefined') {
            var btn = document.getElementById(searchButtonId);
            var hiddenFocus = document.getElementById('__activeSearchId');
            var hiddenCursor = document.getElementById('__activeCursorPos');

            if (hiddenFocus) hiddenFocus.value = boxId;
            if (hiddenCursor) hiddenCursor.value = cursorPos;

            if (btn) {
                btn.click();
            }
        }
    }, 300);
}

// Unified ASP.NET Web Forms UpdatePanel postback handler for modal listeners and cursor/focus restoration
if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
        // Re-attach modal listener after UpdatePanel refreshes
        initModalListeners();

        // Automatically restore focus and precise cursor position after UpdatePanel refreshes
        var hiddenFocus = document.getElementById('__activeSearchId');
        var hiddenCursor = document.getElementById('__activeCursorPos');

        if (hiddenFocus && hiddenFocus.value) {
            var txt = document.getElementById(hiddenFocus.value);
            if (txt) {
                txt.focus();
                var pos = hiddenCursor ? parseInt(hiddenCursor.value) : txt.value.length;
                if (!isNaN(pos) && txt.setSelectionRange) {
                    txt.setSelectionRange(pos, pos);
                }
            }
        }
    });
}
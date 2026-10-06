/**
 * Issue-tracker-archived.js
 * Handles live search debouncing, 3-character threshold, and cursor/focus preservation for Archive.aspx.
 */

var searchTimer = null;

/**
 * Triggers live search asynchronously with a debounce delay once at least 3 characters are typed (or input is cleared).
 * Preserves current cursor position and focus.
 * @param {HTMLInputElement} txtBox - The search text input element.
 */
function triggerArchiveSearch(txtBox) {
    // Clear previous timer while the user is actively typing
    if (searchTimer) {
        window.clearTimeout(searchTimer);
    }

    var keyword = txtBox.value ? txtBox.value.trim() : "";

    // If the user clears the search completely, allow immediate reset
    // If the user types 1 or 2 characters, wait until they reach 3
    if (keyword.length > 0 && keyword.length < 3) {
        return;
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

// Automatically restore focus and precise cursor position after UpdatePanel refreshes
if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
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
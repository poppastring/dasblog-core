function commentManagement(postid, commentid, commentText, httpVerb, action) {
    // Resolve the action key. Defaults preserve historical behaviour:
    //   DELETE -> delete, PATCH -> approve. Pass 'UNAPPROVE' to send a comment back to pending.
    const actionKey = action
        || (httpVerb === 'DELETE' ? 'DELETE' : 'APPROVE');

    // Store comment details for modal
    window.pendingCommentAction = {
        postid: postid,
        commentid: commentid,
        httpVerb: httpVerb,
        actionKey: actionKey
    };

    // Set modal content based on action type
    let modalTitle, modalButton, modalButtonClass;
    switch (actionKey) {
        case 'DELETE':
            modalTitle = 'Confirm Delete';
            modalButton = 'Delete';
            modalButtonClass = 'btn-danger';
            break;
        case 'UNAPPROVE':
            modalTitle = 'Move Back to Pending';
            modalButton = 'Move to Pending';
            modalButtonClass = 'btn-warning';
            break;
        default:
            modalTitle = 'Confirm Approve';
            modalButton = 'Approve';
            modalButtonClass = 'btn-success';
            break;
    }
    const modalBody = commentText;

    // Get or create modal
    let modal = document.getElementById('commentActionModal');
    if (!modal) {
        // Create modal if it doesn't exist
        const modalHtml = `
            <div class="modal fade" id="commentActionModal" tabindex="-1" aria-labelledby="commentActionModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="commentActionModalLabel"></h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body" id="commentActionModalBody"></div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <button type="button" class="btn" id="confirmCommentActionButton"></button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        document.body.insertAdjacentHTML('beforeend', modalHtml);
        modal = document.getElementById('commentActionModal');
    }

    // Update modal content
    document.getElementById('commentActionModalLabel').textContent = modalTitle;
    document.getElementById('commentActionModalBody').textContent = modalBody;
    const confirmButton = document.getElementById('confirmCommentActionButton');
    confirmButton.textContent = modalButton;
    confirmButton.className = `btn ${modalButtonClass}`;

    // Remove existing event listener and add new one
    const newConfirmButton = confirmButton.cloneNode(true);
    confirmButton.parentNode.replaceChild(newConfirmButton, confirmButton);

    newConfirmButton.addEventListener('click', function() {
        executeCommentAction();
    });

    // Show modal
    const bsModal = new bootstrap.Modal(modal);
    bsModal.show();
}

function executeCommentAction() {
    const action = window.pendingCommentAction;
    if (!action) return;

    const oReq = new XMLHttpRequest();
    const basePath = document.querySelector('base')?.getAttribute('href') || '/';
    let url = basePath + 'admin/post/' + action.postid + '/comments/' + action.commentid;
    if (action.actionKey === 'UNAPPROVE') {
        url += '/unapprove';
    }

    oReq.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            console.log('Comment action completed successfully');

            // Close modal
            const modal = document.getElementById('commentActionModal');
            const bsModal = bootstrap.Modal.getInstance(modal);
            if (bsModal) {
                bsModal.hide();
            }

            // Set success message in session storage
            let successMessage;
            switch (action.actionKey) {
                case 'DELETE':
                    successMessage = 'Comment deleted successfully!';
                    break;
                case 'UNAPPROVE':
                    successMessage = 'Comment moved back to pending.';
                    break;
                default:
                    successMessage = 'Comment approved successfully!';
                    break;
            }

            console.log('Setting sessionStorage with message:', successMessage);
            sessionStorage.setItem('commentActionSuccess', successMessage);

            // Verify it was set
            console.log('Verification - sessionStorage value:', sessionStorage.getItem('commentActionSuccess'));

            // Small delay to ensure sessionStorage is committed before reload
            setTimeout(function() {
                console.log('Reloading page...');
                window.location.reload();
            }, 100);
        } else if (this.readyState == 4) {
            console.error('Comment action failed with status:', this.status);
        }
    };

    oReq.open(action.httpVerb, url);
    oReq.send();

    // Clear pending action
    window.pendingCommentAction = null;
}

function linkToUser(emailAddress, linkAbility) {
    if (linkAbility === "disabled") {
        alert("You must complete the current operation (create, edit or delete user), before you can select another");
        return;
    }
    location.href = emailAddress;
}

function showLastUserError(showError) {
    if (showError) {
        alert("You can't delete your own user record or reduce its privileges");
        return;
    }
    window.maintenanceForm.submit();
}

(function () {
    var STORAGE_KEY = 'dasblog-theme';

    function getStoredTheme() {
        try { return localStorage.getItem(STORAGE_KEY); } catch (e) { return null; }
    }

    function setStoredTheme(theme) {
        try { localStorage.setItem(STORAGE_KEY, theme); } catch (e) { /* ignore */ }
    }

    function resolveTheme(theme) {
        if (!theme || theme === 'auto') {
            return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
        }
        return theme;
    }

    function applyTheme(theme) {
        document.documentElement.setAttribute('data-bs-theme', resolveTheme(theme));
    }

    function updateActiveIndicator(selected) {
        var options = document.querySelectorAll('.dasblog-theme-option');
        options.forEach(function (btn) {
            var check = btn.querySelector('.dasblog-theme-check');
            var isActive = btn.getAttribute('data-bs-theme-value') === selected;
            btn.classList.toggle('active', isActive);
            if (check) {
                check.classList.toggle('d-none', !isActive);
            }
        });
    }

    function init() {
        var current = getStoredTheme() || 'auto';
        applyTheme(current);
        updateActiveIndicator(current);

        document.querySelectorAll('.dasblog-theme-option').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var theme = btn.getAttribute('data-bs-theme-value');
                setStoredTheme(theme);
                applyTheme(theme);
                updateActiveIndicator(theme);
            });
        });

        var media = window.matchMedia('(prefers-color-scheme: dark)');
        if (media.addEventListener) {
            media.addEventListener('change', function () {
                var stored = getStoredTheme();
                if (!stored || stored === 'auto') {
                    applyTheme('auto');
                }
            });
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
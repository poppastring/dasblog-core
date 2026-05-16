/**
 * DasBlog Common UI Utilities
 * Shared functionality for alerts, modals, and form interactions
 */

var DasBlog = DasBlog || {};

DasBlog.UI = (function() {
    'use strict';

    /**
     * Show a success alert from sessionStorage
     * @param {string} storageKey - The sessionStorage key containing the message
     * @param {string} alertId - The ID of the alert element
     * @param {string} messageId - The ID of the message span element
     */
    function showSessionAlert(storageKey, alertId, messageId) {
        const message = sessionStorage.getItem(storageKey);
        
        if (message) {
            const alertElement = document.getElementById(alertId);
            const messageElement = document.getElementById(messageId);

            if (alertElement && messageElement) {
                messageElement.textContent = message;
                alertElement.classList.remove('d-none');
                alertElement.classList.add('show');

                // Scroll to alert
                setTimeout(function() {
                    alertElement.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
                }, 100);

                // Auto-dismiss after 5 seconds
                setTimeout(function() {
                    if (typeof bootstrap !== 'undefined') {
                        const bsAlert = bootstrap.Alert.getOrCreateInstance(alertElement);
                        bsAlert.close();
                    }
                }, 5000);
            }

            // Clear from storage
            sessionStorage.removeItem(storageKey);
        }
    }

    /**
     * Initialize a confirmation modal that triggers a hidden submit button
     * @param {string} modalId - The ID of the modal element
     * @param {string} confirmButtonId - The ID of the confirm button in the modal
     * @param {string} hiddenButtonId - The ID of the hidden submit button
     */
    function initConfirmModal(modalId, confirmButtonId, hiddenButtonId) {
        const confirmButton = document.getElementById(confirmButtonId);
        const hiddenButton = document.getElementById(hiddenButtonId);
        const modal = document.getElementById(modalId);

        if (confirmButton && hiddenButton && modal) {
            confirmButton.addEventListener('click', function() {
                const modalInstance = bootstrap.Modal.getInstance(modal);
                if (modalInstance) {
                    modalInstance.hide();
                }

                setTimeout(function() {
                    hiddenButton.click();
                }, 300);
            });
        }
    }

    /**
     * Save accordion state to sessionStorage
     * @param {string} storageKey - The key to store accordion state under
     * @param {NodeList} accordionItems - NodeList of accordion collapse elements
     */
    function saveAccordionState(storageKey, accordionItems) {
        const openAccordions = [];
        accordionItems.forEach(function(item) {
            if (item.classList.contains('show')) {
                openAccordions.push(item.id);
            }
        });
        sessionStorage.setItem(storageKey, JSON.stringify(openAccordions));
    }

    /**
     * Restore accordion state from sessionStorage
     * @param {string} storageKey - The key where accordion state is stored
     */
    function restoreAccordionState(storageKey) {
        const savedState = sessionStorage.getItem(storageKey);
        if (savedState) {
            try {
                const openAccordions = JSON.parse(savedState);
                openAccordions.forEach(function(accordionId) {
                    const accordionElement = document.getElementById(accordionId);
                    if (accordionElement) {
                        new bootstrap.Collapse(accordionElement, {
                            show: true
                        });
                    }
                });
            } catch (e) {
                console.error('Error restoring accordion state:', e);
            }
        }
    }

    /**
     * Initialize accordion state tracking
     * @param {string} storageKey - The key to store accordion state under
     * @param {NodeList} accordionItems - NodeList of accordion collapse elements
     */
    function initAccordionTracking(storageKey, accordionItems) {
        // Restore state on load
        restoreAccordionState(storageKey);

        // Track changes
        accordionItems.forEach(function(item) {
            item.addEventListener('shown.bs.collapse', function() {
                saveAccordionState(storageKey, accordionItems);
            });
            item.addEventListener('hidden.bs.collapse', function() {
                saveAccordionState(storageKey, accordionItems);
            });
        });
    }

    /**
     * Auto-dismiss an alert after a specified time
     * @param {string} alertId - The ID of the alert element
     * @param {number} delay - Delay in milliseconds (default: 5000)
     */
    function autoDismissAlert(alertId, delay) {
        delay = delay || 5000;
        const alertElement = document.getElementById(alertId);
        
        if (alertElement) {
            setTimeout(function() {
                if (typeof bootstrap !== 'undefined') {
                    const bsAlert = bootstrap.Alert.getOrCreateInstance(alertElement);
                    bsAlert.close();
                }
            }, delay);
        }
    }

    /**
     * Initialize the hero image picker modal for the blog post editor.
     * Reuses the existing /api/image/list and /api/image/upload endpoints
     * so the editor has a single image pipeline.
     */
    function initHeroImagePicker() {
        const modalEl = document.getElementById('heroImagePickerModal');
        if (!modalEl) {
            return;
        }

        let modalInstance = null;

        function $(id) { return document.getElementById(id); }

        function ensureModal() {
            if (!modalInstance && typeof bootstrap !== 'undefined' && bootstrap.Modal) {
                modalInstance = bootstrap.Modal.getOrCreateInstance(modalEl);
            }
            return modalInstance;
        }

        function applySelection(url) {
            if (!url) return;
            const urlInput = $('HeroImageUrl');
            const altInput = $('HeroImageAlt');
            const previewWrapper = $('HeroImagePreviewWrapper');
            const previewImg = $('HeroImagePreview');
            if (urlInput) urlInput.value = url;
            if (previewImg) {
                previewImg.src = url;
                previewImg.alt = altInput ? altInput.value : '';
            }
            if (previewWrapper) previewWrapper.classList.remove('d-none');
            const m = ensureModal();
            if (m) m.hide();
        }

        function renderImageButton(grid, fullUrl, label) {
            const col = document.createElement('div');
            col.className = 'col';
            const btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'btn p-1 w-100 border h-100 d-flex flex-column align-items-center hero-picker-in-post-item';
            btn.title = label;
            btn.dataset.heroUrl = fullUrl;
            btn.innerHTML =
                '<img src="' + fullUrl + '" alt="" class="img-fluid mb-1" style="max-height: 100px; object-fit: contain; pointer-events: none;" />' +
                '<small class="text-truncate w-100" style="pointer-events: none;">' + label + '</small>';
            col.appendChild(btn);
            grid.appendChild(col);
        }

        function getJoditInstance() {
            // The Jodit init script attaches the editor instance both on
            // window.dasBlogEditor and on the underlying textarea element.
            if (typeof window !== 'undefined' && window.dasBlogEditor) {
                return window.dasBlogEditor;
            }
            const textarea = $('mytextarea');
            if (textarea && textarea.jodit) return textarea.jodit;
            // Jodit v4 maintains a static instances map keyed by element.
            if (typeof window !== 'undefined' && window.Jodit && window.Jodit.instances) {
                const inst = window.Jodit.instances['mytextarea']
                    || (textarea && window.Jodit.instances[textarea.id]);
                if (inst) return inst;
            }
            return null;
        }

        function getCurrentPostHtml() {
            const jodit = getJoditInstance();
            if (jodit && typeof jodit.getEditorValue === 'function') {
                try { return jodit.getEditorValue() || ''; } catch (e) { /* ignore */ }
            }
            const textarea = $('mytextarea');
            return textarea ? (textarea.value || '') : '';
        }

        function extractPostImages() {
            const html = getCurrentPostHtml();
            if (!html) return [];
            const parser = new DOMParser();
            const doc = parser.parseFromString(html, 'text/html');
            const imgs = doc.querySelectorAll('img[src]');
            const seen = new Set();
            const results = [];
            imgs.forEach(function (img) {
                const src = (img.getAttribute('src') || '').trim();
                if (!src || seen.has(src)) return;
                seen.add(src);
                let label = src;
                const lastSlash = src.lastIndexOf('/');
                if (lastSlash >= 0 && lastSlash < src.length - 1) {
                    label = src.substring(lastSlash + 1);
                }
                results.push({ url: src, label: label });
            });
            return results;
        }

        function renderInPost() {
            const inPostGrid = $('heroPickerInPostGrid');
            const inPostStatus = $('heroPickerInPostStatus');
            if (!inPostGrid) return;
            inPostGrid.innerHTML = '';
            const images = extractPostImages();
            if (images.length === 0) {
                if (inPostStatus) {
                    inPostStatus.textContent = 'No images found in the current post body. Browse the server library or paste a remote URL below.';
                }
                return;
            }
            if (inPostStatus) {
                inPostStatus.textContent = images.length + ' image' + (images.length === 1 ? '' : 's') + ' found in this post.';
            }
            images.forEach(function (img) {
                renderImageButton(inPostGrid, img.url, img.label);
            });
        }

        function openServerBrowser() {
            const jodit = getJoditInstance();
            if (!jodit || !jodit.filebrowser || typeof jodit.filebrowser.open !== 'function') {
                window.alert('Server image browser is not available yet. Please wait for the editor to finish loading.');
                return;
            }
            const m = ensureModal();
            if (m) m.hide();
            try {
                jodit.filebrowser.open(function (data) {
                    if (!data) return;
                    const files = data.files || data;
                    if (!files || !files.length) return;
                    const first = files[0];
                    let url = '';
                    if (typeof first === 'string') {
                        url = first;
                    } else if (first && first.url) {
                        url = first.url;
                    } else if (first && first.name) {
                        url = (data.baseurl || '') + first.name;
                    }
                    if (url) applySelection(url);
                }, false);
            } catch (e) {
                window.alert('Unable to open the server image browser: ' + e.message);
            }
        }

        function applyRemote() {
            const input = $('heroPickerRemoteUrl');
            if (!input) return;
            const value = (input.value || '').trim();
            if (!value) return;
            applySelection(value);
            input.value = '';
        }

        // Refresh "In this post" each time the modal opens.
        modalEl.addEventListener('shown.bs.modal', renderInPost);

        // Event delegation so the handlers survive any DOM rebuilds / timing quirks.
        document.addEventListener('click', function (ev) {
            const target = ev.target;
            if (!target || target.nodeType !== 1) return;

            if (target.closest('#heroPickerBrowseServer')) {
                ev.preventDefault();
                openServerBrowser();
                return;
            }
            if (target.closest('#heroPickerRemoteApply')) {
                ev.preventDefault();
                applyRemote();
                return;
            }
            const inPostBtn = target.closest('.hero-picker-in-post-item');
            if (inPostBtn) {
                ev.preventDefault();
                applySelection(inPostBtn.dataset.heroUrl || '');
            }
        });

        // Pressing Enter in the remote URL field should apply it.
        document.addEventListener('keydown', function (ev) {
            if (ev.key !== 'Enter') return;
            const target = ev.target;
            if (target && target.id === 'heroPickerRemoteUrl') {
                ev.preventDefault();
                applyRemote();
            }
        });
    }

    // Auto-init the hero picker when present on the page.
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initHeroImagePicker);
    } else {
        initHeroImagePicker();
    }

    // Public API
    return {
        showSessionAlert: showSessionAlert,
        initConfirmModal: initConfirmModal,
        saveAccordionState: saveAccordionState,
        restoreAccordionState: restoreAccordionState,
        initAccordionTracking: initAccordionTracking,
        autoDismissAlert: autoDismissAlert,
        initHeroImagePicker: initHeroImagePicker
    };
})();

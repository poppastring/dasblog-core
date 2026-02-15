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
                alertElement.style.display = 'block';
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

    // Public API
    return {
        showSessionAlert: showSessionAlert,
        initConfirmModal: initConfirmModal,
        saveAccordionState: saveAccordionState,
        restoreAccordionState: restoreAccordionState,
        initAccordionTracking: initAccordionTracking,
        autoDismissAlert: autoDismissAlert
    };
})();

document.addEventListener('DOMContentLoaded', async () => {
    const modalElement = document.getElementById('settingsModal');
    if (modalElement) {
        settingsModal = new bootstrap.Modal(modalElement);
        settingsModal.show();
    }
});

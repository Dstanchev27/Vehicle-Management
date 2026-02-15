$(document).ready(function () {
    const deleteServiceButtons = document.querySelectorAll('.btn-delete-service');
    const serviceIdInput = document.getElementById('serviceId');
    const serviceNameSpan = document.getElementById('serviceName');
    const deleteServiceForm = document.getElementById('deleteServiceForm');

    deleteServiceButtons.forEach(button => {
        button.addEventListener('click', () => {
            const serviceId = button.getAttribute('data-service-id');
            const serviceName = button.getAttribute('data-service-name');

            serviceIdInput.value = serviceId;
            serviceNameSpan.innerText = serviceName;
        });
    });

    deleteServiceForm.addEventListener('submit', (event) => {
        const serviceId = serviceIdInput.value;

        console.log(`Service with ID ${serviceId} is being deleted.`);
    });
});
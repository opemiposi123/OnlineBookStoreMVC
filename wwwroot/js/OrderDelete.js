$(document).ready(function () {
        $('.delete-order').on('click', function () {
            var orderId = $(this).data('id');
            var button = $(this);

            if (confirm('Are you sure you want to delete this order?')) {
                $.ajax({
                    url: '@Url.Action("DeleteOrder", "Order")/' + orderId,
                    type: 'DELETE',
                    success: function (result) {
                        // Remove the order row from the table
                        button.closest('tr').remove();
                    },
                    error: function (xhr, status, error) {
                        alert('Failed to delete order: ' + error);
                    }
                });
            }
        });
    });

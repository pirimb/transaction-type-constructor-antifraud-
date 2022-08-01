// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $.ajax({
        url: '/Home/GetUserNameFromSession',
        type: 'GET',
        dataType: 'text',
        success: function (data) {
            $('#username').text(data);
        },
    });
    $("#inputForGettingTrnType").keypress(function (event) {
        var inputValue = event.which;
        if (!((inputValue >= 48 && inputValue <= 57) ||  // a-z
            (inputValue == 32))) {                      // space
            event.preventDefault();
        }
    });
});

var table = $('#mainDataTable').DataTable({
    initComplete: function () {
        this.api()
            .columns()
            .every(function () {
                var column = this;
                var select = $('<select style="width:100%"><option value=""></option></select>')
                    .appendTo($(column.footer()).empty())
                    .on('change', function () {
                        var val = $.fn.dataTable.util.escapeRegex($(this).val());
                        column.search(val ? '^' + val + '$' : '', true, false).draw();
                    });

                column
                    .data()
                    .unique()
                    .sort()
                    .each(function (d, j) {
                        select.append('<option value="' + d + '">' + d + '</option>');
                    });
            });
    },
    order: [],
    "language": {
        "decimal": "",
        "emptyTable": "Məlumat yoxdur!",
        "info": "_TOTAL_ elementdən _START_ - _END_ arası",
        "infoEmpty": "Boş",
        "infoFiltered": "(filtered from _MAX_ total entries)",
        "infoPostFix": "",
        "thousands": ",",
        "lengthMenu": " _MENU_ element göstər",
        "loadingRecords": "Loading...",
        "processing": "Processing...",
        "search": "Axtar:",
        "zeroRecords": "Axtarış üzrə məlumat tapılmadı",
        "paginate": {
            "first": "First",
            "last": "Last",
            "next": "Sonrakı",
            "previous": "Öncəki"
        },
        "aria": {
            "sortAscending": ": activate to sort column ascending",
            "sortDescending": ": activate to sort column descending"
        }
    },
    "scrollX": false, "ordering": true,
    searching: true, paging: true, info: true,
});


$("#debet_hesab").on("input", function () {
    var str = $("#debet_hesab").val();
    $.ajax({
        url: "/Home/CheckAccount",
        type: "GET",
        data: { account: str },
        dataType: 'text',
        success: function (result) {
            if (result == 0) {
                $("#debet_hesab").addClass("is-invalid")
                $("#debet_hesab").removeClass("is-valid")
            } else {
                $("#debet_hesab").removeClass("is-invalid")
                $("#debet_hesab").addClass("is-valid")
            }
        }
    });
});

$("#credit_hesab").on("input", function () {
    var str = $("#credit_hesab").val();
    $.ajax({
        url: "/Home/CheckAccount",
        type: "GET",
        data: { account: str },
        dataType: 'text',
        success: function (result) {
            if (result == 0) {
                $("#credit_hesab").addClass("is-invalid")
                $("#credit_hesab").removeClass("is-valid")
            } else {
                $("#credit_hesab").removeClass("is-invalid")
                $("#credit_hesab").addClass("is-valid")
            }
        }
    });
});

$("#debet_owner").on("input", function () {
    var str = $("#debet_owner").val();
    $.ajax({
        url: "/Home/CheckOwner",
        type: "GET",
        data: { ownerNumber: str },
        dataType: 'text',
        success: function (result) {
            if (result == 0) {
                $("#debet_owner").addClass("is-invalid")
                $("#debet_owner").removeClass("is-valid")
            } else {
                $("#debet_owner").removeClass("is-invalid")
                $("#debet_owner").addClass("is-valid")
            }
        }
    });
});

$("#credit_owner").on("input", function () {
    var str = $("#credit_owner").val();
    $.ajax({
        url: "/Home/CheckOwner",
        type: "GET",
        data: { ownerNumber: str },
        dataType: 'text',
        success: function (result) {
            if (result == 0) {
                $("#credit_owner").addClass("is-invalid")
                $("#credit_owner").removeClass("is-valid")
            } else {
                $("#credit_owner").removeClass("is-invalid")
                $("#credit_owner").addClass("is-valid")
            }
        }
    });
});

$("#btnForGettingTrnType").click(function () {
    var str = $("#inputForGettingTrnType").val();
    if (str.length != 0) {
        $.ajax({
            url: "/Home/GetTrnType",
            type: "GET",
            data: { trnType: str },
            dataType: 'text',
            success: function (result) {
                alert(result);
            }
        });
    }
});




var url = window.location;

$('ul.nav-sidebar a').filter(function () {
    return this.href == url;
}).addClass('active');


$(document).ready(function () {
    $("#btnForOpenModalTrn").click(function () {
        $("#myModal").modal('show');
    });
});
$(document).ready(function () {
    $("#modalCancelBtn").click(function () {
        $("#myModal").modal('hide');
    });
});
$(document).ready(function () {
    $(".close").click(function () {
        $("#myModal").modal('hide');
    });
});

$(document).ready(function (e) {
    $(document).on("click", ".delete-modal", function (e) {
        var delete_id = $(this).attr('data-value');
        $('button[name="delete_user"]').val(delete_id);
    });
});



//$(document).ready(function () {
//    $(".editTrnBtnModal").click(function () {
//        str = (".editTrnBtnModal").val()
//        $("."+str).modal('show');
//    });
//});
//$(document).ready(function () {
//    $("#modalCancelBtnEdit").click(function () {
//        $(".editTrnModal").modal('hide');
//    });
//});
//$(document).ready(function () {
//    $(".close").click(function () {
//        $(".editTrnModal").modal('hide');
//    });
//});

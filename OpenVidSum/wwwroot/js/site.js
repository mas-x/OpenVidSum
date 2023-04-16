$("#btnSummarize").click((e) => {
    e.preventDefault();
    var formData = new FormData();
    formData.append("videoLink", $("#txtVideoLink").val());

    $("#videoSummary").empty();

    $.ajax({
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        beforeSend: () => {
            $("#frmVideo").fadeOut(100);
            hideOrShowFlexElement("#spinner", true);
        },
        complete: () => {
            $("#frmVideo").fadeIn(100);
            hideOrShowFlexElement("#spinner", false);
        },
        success: (result) => {
            if (result.responses) {              
                $("#videoSummary").append("<h1 class='mt-3'>Summary</h1>");
                result.responses.forEach((response) => {
                    $("#videoSummary").append(`<div>${response}</div>`)
                })
            }
        }
    });
});

const hideOrShowFlexElement = (e, show) => {
    if (show) {
        $(e).addClass("d-flex");
        $(e).removeClass("d-none");
    }
    else {
        $(e).removeClass("d-flex");
        $(e).addClass("d-none");
    }
}

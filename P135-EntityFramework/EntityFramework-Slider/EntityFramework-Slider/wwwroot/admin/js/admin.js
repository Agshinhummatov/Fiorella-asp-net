$(function () {

    $(document).on("click", ".slider-status", function () {

        let sliderId = $(this).parent().attr("data-id");
        let changeElem = $(this);
        let data = { id: sliderId }

        console.log("test")

        $.ajax({
            url: "slider/setstatus",
            type: "Post",
            data: data,
            success: function (res) {
                if (res) {
                    $(changeElem).removeClass("active-status");
                    $(changeElem).addClass("deActive-status");
                } else {
                    $(changeElem).addClass("active-status");
                    $(changeElem).removeClass("deActive-status");
                }
            }

        })
    })


    $(document).on("submit", "#category-delete-form", function (e) {
        e.preventDefault();
        let categoryId = $(this).attr("data-id");
        let deletedElem = $(this);
        let data = { id: categoryId }


        $.ajax({
            url: "category/softdelete",
            type: "Post",
            data: data,
            success: function (res) {
                $(deletedElem).parent().parent().remove();
            }

        })
    })

    $(document).on("click", ".trash-icon i", function (e) {

        let imageId = $(this).parent().attr("data-id");
        let deletedElem = $(this).parent();
        let data = { id: imageId };

        $.ajax({
            url: "/Admin/Product/DeleteProductImage",
            type: "Post",
            data: data,
            success: function (res) {
                if (res) {
                    $(deletedElem).remove();
                } else {
                    alert("Product images must be min 1")
                }
               
            }

        })
    })
})
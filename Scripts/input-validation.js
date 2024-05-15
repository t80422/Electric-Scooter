$(document).ready(function () {
    //限制只能輸入英文大小寫和數字
    $('.alpha-numeric').on('input', function () {
        $(this).val($(this).val().replace(/[^a-zA-Z0-9]/g, ''));
    });

    // 限制只能輸入中文和英文大小寫
    var composing = false;

    $('.chinese-alpha').on('compositionstart', function () {
        composing = true;
    });

    $('.chinese-alpha').on('compositionend', function () {
        composing = false;
        // 檢查輸入後的內容，並移除不符合條件的字符
        $(this).val($(this).val().replace(/[^\u4e00-\u9fa5a-zA-Z]/g, ''));
    });

    $('.chinese-alpha').on('input', function () {
        if (!composing) {
            $(this).val($(this).val().replace(/[^\u4e00-\u9fa5a-zA-Z]/g, ''));
        }
    });
})
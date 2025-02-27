//var modalOk = function ({ title, message, okCallback, static = false, cancelCallback = null }) {

//    const msgDiv = `<div id="okDialog" class="text-center mb-0 p-4">${message || ''}</div>`;

//    const buttons = [{
//        label: 'OK',
//        className: "btn-primary",
//        callback: okCallback ? () => okCallback() : () => { }
//    }];

//    if (cancelCallback) {
//        buttons.push({
//            label: 'Cancel',
//            className: "btn-secondary",
//            callback: () => { }
//        });
//    }

//    const dialog = bootbox.dialog({
//        title: title || 'Title',
//        message: msgDiv,
//        centerVertical: true,
//        closeButton: true,
//        backdrop: static ? true : 'static',
//        buttons: buttons
//    });
//}

//var hideModalOk = function () {
//    dialog.modal('hide');
//}
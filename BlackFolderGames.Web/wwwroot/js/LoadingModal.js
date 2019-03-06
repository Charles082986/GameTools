﻿class LoadingModal {
    constructor() {
        this.__modal = $('<div class="modal hide" id="pleaseWaitDialog" data-backdrop="static" data-keyboard="false"><div class="modal-header"><h1>Loading...</h1></div><div class="modal-body"><div class="progress progress-striped active"><div class="bar" style="width: 100%;"></div></div></div></div>');
    }

    Show() {
        this.__modal.modal();
    }

    Hide() {
        this.__modal.modal('hide');
    }
};

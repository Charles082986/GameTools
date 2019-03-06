class SearchBox {
    constructor(selector, searchDelay, searchFunction, ...searchArgs) {
        this.Selector = selector;
        this.SearchDelay = searchDelay;
        this.SearchFunction = searchFunction;
        this.__arguments = searchArgs;
        this.__timeoutKey = null;
        this.__previousSearchTerm = '';
        $(selector).on('keypress', queueSearch);
    }

    queueSearch = function (event) {
        let searchTerm = $(event.target).val();
        if (searchTerm !== this.__previousSearchTerm) {
            this.__previousSearchTerm = searchTerm;
            clearTimeout(this.timeoutKey);
            this.timeoutKey = setTimeout(this.SearchFunction, this.SearchDelay, searchTerm, ... this.__arguments);
        }
    };
}
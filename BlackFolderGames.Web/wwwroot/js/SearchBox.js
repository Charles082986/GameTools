class SearchBox {
    constructor(selector, searchDelay, searchFunction) {
        this.Selector = selector;
        this.SearchDelay = searchDelay;
        this.SearchFunction = searchFunction;
        this.__timeoutKey = null;
        this.__previousSearchTerm = '';
        $(selector).on('input', null, this, this.queueSearch);
    }

    queueSearch(event) {
        console.log(event.data.SearchFunction);
        console.log(event.data.SearchDelay);
        let searchTerm = this.value;
        console.log('queueing search, searchTerm: ' + searchTerm ? searchTerm : '<none>');
        if (searchTerm !== event.data.__previousSearchTerm) {
            event.data.__previousSearchTerm = searchTerm;
            clearTimeout(event.data.__timeoutKey);
            event.data.__timeoutKey = setTimeout(event.data.SearchFunction, event.data.SearchDelay, searchTerm);
        }
    };
}
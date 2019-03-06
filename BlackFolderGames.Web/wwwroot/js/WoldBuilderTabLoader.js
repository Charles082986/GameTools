class CardTabFiller {
    constructor(cardBuilder) {
        this.CardBuilder = cardBuilder;
    }

    FillTab(tabSelector, content, countSelector = null) {
        let count = 0;
        $(tabSelector).empty();
        let cards = [];
        for (let item of content) {
            count++;
            cards.push(this.CardBuilder.BuildCard(item));
        }
        $(tabSelector.append(cards.join('')));
        if (countSelector) {
            $(countSelector).text(cardCount);
        }
    }
};
    
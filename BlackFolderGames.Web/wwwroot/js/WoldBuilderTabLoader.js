/**
 * @param {string} [name] The name of the tab to fill.
 * @param {object[]} [content] The data collection used to fill the tab. 
 * @param {bool} [group] Flag indicating if items are grouped by UserName.
 */
let fillTab = (name, content, group = false) => {
    if (content && content.length > 0) {
        let cards = [];
        let cardCount = 0;
        if (group) {
            for (let user of content) {
                cards.push('<h4>' + user.userName + '</h4>');
                for (let item of user.regionSets) {
                    cardCount++;
                    cards.push(buildBootstrapCard('', 'col-md-3', item.name, item.imageURL, item.description, "/World/" + item.regionSetId, "Go"));
                }
            }
        } else {
            for (let item of content) {
                cardCount++;
                cards.push(buildBootstrapCard('', 'col-md-3', item.name, item.imageURL, item.description, "/World/" + item.regionSetId, "Go"));
            }
        }
        $('#' + name).append(cards.join(''));
        $('#' + name + '-count').text(cardCount);
    } else {
        $('#' + name + '-count').text('0');
        if (name === 'owned') {
            let createNewLink = '<a href="/WorldBuilder/World/Create">Create a Region Set</a>';
            $('#' + name).append('<h4 class="col-md-12">You don\'t have any active Region Sets.  Would you like to ' + createNewLink + '?</h4>');
        } else {
            $('#' + name).append('<h4 class="col-md-12">There are no items available to view.</h4>');
        }
    }
};

let bindData = (data) => {
    console.log(data);
    if (data) {
        fillTab('owned', data.ownedRegionSets);
        fillTab('shared', data.editableRegionSets);
        fillTab('browse', data.viewableRegionSets);
    }
};

let fetchRegionSetData = (searchTerm) => {
    if (searchTerm && searchTerm.length > 0) {
        $.get("/WorldBuilder/World/RegionSetSearch?searchTerm=" + searchTerm).done(bindData);
    }
    else {
        $.get("/WorldBuilder/World/RegionSetSearch").done(bindData);
    }
}
    
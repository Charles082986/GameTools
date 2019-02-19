/**
 * @param {string} [id] The element id.
 * @param {string} [classes] The element css classes.
 * @param {string} [title] The element title.
 * @param {string} [image] The element image url.
 * @param {string} [text] The text of the element.
 * @param {string} [link] The link target of the card.
 * @param {string} [buttonText] the button text of the card.
 * @return {string} The completed Bootstrap Card object to be added to the UI.
 * */
let buildBootstrapCard = (id, classes, title, image, text, link, buttonText) => {
    return "<div id='" + id + "' class='card " + classes + "'><a href='" + link + "'><img class='card-img-top img-responsive' src='" + image + "'></a><div class='card-body'><h4 class='card-title'>" + title + "</h4><p class='card-text'>" + text + "</p><a href='" + link + "' class='btn btn-default'>View »</a></div></div>";
};
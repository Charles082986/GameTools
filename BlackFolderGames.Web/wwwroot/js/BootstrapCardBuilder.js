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
    let idSection = '';
    if (id || id.length === 0) {
        idSection = `id='${id}'`;
    }
    return `<div ${idSection} class='card border ${classes}'><a href='${link}'><div class="card-image"><img class='card-img-top img-responsive full-width' src='${image}'/></div><div class='card-body'><h4 class='card-title caption text-center'>${title}</h4></a></div></div>`;
};
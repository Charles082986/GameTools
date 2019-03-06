class BoostrapCardBuilder {
    constructor(idCallback, classCallback, linkCallback, imageCallback, titleCallback, descriptionCallback, buttonTextCallback) {
        this.GetId = idCallback;
        this.GetClasses = classCallback;
        this.GetLink = linkCallback;
        this.GetImageSource = imageCallback;
        this.GetTitle = titleCallback;
        this.GetDescription = descriptionCallback;
        this.GetButtonText = buttonTextCallback;
    }

    BuildCard(context) {
        let id = this.GetId(context);
        let classes = this.GetClasses(context);
        let link = this.GetLink(context);
        let image = this.GetImageSource(context);
        let title = this.GetTitle(context);

        let idSection = (id && id.length !== 0) ? `id='${id}'` : '';
        return `<div ${idSection} class='card hover-borders ${classes}'><a href='${link}'><div class="card-image"><img class='card-img-top img-responsive full-width' src='${image}'/></div><div class='card-body'><h4 class='card-title caption text-center'>${title}</h4></a></div></div>`;
    }
};
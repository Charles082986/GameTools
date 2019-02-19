let handleDynamicImageSource = (event) => {
    let imageSelector = $(event.target).prop('dynamic-image-source-for');
    let imageSource = $(event.target).text();
    $(imageSelector).prop('src', imageSource);
};
$('.dynamic-image-source').on('changed', handleDynamicImageSource);
    
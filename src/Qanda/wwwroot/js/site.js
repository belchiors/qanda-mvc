// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const editMde = new SimpleMDE({
    element: document.querySelector('.md-editor'),
    initialValue: '',
    hideIcons: ['guide', 'fullscreen', 'side-by-side'],
    status: false,
    autosave: {
        enabled: false
    }
});
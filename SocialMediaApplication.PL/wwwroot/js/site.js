// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let btn = document.getElementById("post-text");
let modal = document.getElementById("modal-div");
let createPostForm = document.getElementById("form-container");


modal.onclick = function () {
    createPostForm.classList.toggle("show");
    $("#form-container").html("")
    modal.classList.toggle("show");
};
$("#post-text").click(function () {
    modal.classList.add("show");
    createPostForm.classList.add("show");
    fetch("/Home/GetCreateForm")
        .then(res => res.text())
        .then(result => {

            $("#form-container").html(result);
            loadFormEvents();
            bindCreatePostForm();
        });
});

function loadFormEvents() {
    let closeButton = document.getElementById("close");
    closeButton.onclick = function () {
        createPostForm.classList.toggle("show");
        $("#form-container").html("");
        modal.classList.toggle("show");
    };
    const imgInput = document.getElementById('imgInput');
    const uploadBtn = document.getElementById('uploadBtn');
    const image = document.getElementById('preview');

    uploadBtn.addEventListener('click', () => {
        imgInput.click();
    });

    imgInput.addEventListener('change', () => {
        if (imgInput.files && imgInput.files[0]) {
            const file = imgInput.files[0];
            preview.src = URL.createObjectURL(file);
            preview.style.display = 'block';
            preview.onload = () => {
                URL.revokeObjectURL(preview.src);
            };
        }
        else {
            preview.src = '';
            preview.style.display = 'none';
        }
    });
};

let postsContainer = document.getElementById("all-posts");

$(document).ready(function () {
    
    bindCreatePostForm();

});

function bindCreatePostForm() {
    $("#create-post").validate({

        submitHandler: function (form) {
            let formData = new FormData(form);
            fetch("/Home/AddPost", {
                method: "POST",
                body: formData
            })
                .then(res => res.text())
                .then(result => {
                    if (result.includes("create-post")) {

                        $("#form-container").html(result);
                        bindCreatePostForm();
                    }
                    else {
                        createPostForm.classList.toggle("show");
                        $("#form-container").html("");
                        modal.classList.toggle("show");
                        $("#all-posts").prepend(result);
                    }
                });
            return false;
        }
    });
}

document.addEventListener("click", function (e) {
    if (e.target.classList.contains("setting-points-container")) {
        let postUserSettings = document.getElementById(`post-user-settings-${e.target.dataset.postId}`);
        let postFriendSettings = document.getElementById(`post-friend-settings-${e.target.dataset.postId}`);
        if (postUserSettings != null && postUserSettings.classList.contains("show")) {
            postUserSettings.classList.toggle("show");
        }
        else if (postFriendSettings != null && postFriendSettings.classList.contains("show")) {
            postFriendSettings.classList.toggle("show");
        }
        else {
            fetch("/Home/checkPostPrivilege", {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(
                    e.target.dataset.postId
                )
            })
                .then(res => res.json())
                .then(result => {
                    console.log(result.success);
                    console.log(result.IsPostCreator);
                    if (result.success === true) {
                        if (result.isPostCreator === true) {
                            postUserSettings.classList.add("show");
                        }
                        else {
                            postFriendSettings.classList.add("show");
                        }
                    }

                });
        }
    }
    else if (e.target.classList.contains("Delete-Post")) {
        let postUserSettings = document.getElementById(`post-user-settings-${e.target.dataset.postId}`);
        let currentPost = document.getElementsByClassName(`post-${e.target.dataset.postId}`)[0];
        let parent = currentPost.parentNode;
        let nextSibling = currentPost.nextSibling;
        currentPost.remove();
        fetch("/Home/DeletePost", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(
                e.target.dataset.postId
            )
        })
            .then(res => res.json())
            .then(result => {
                if (result.success === false) {
                    parent.insertBefore(post, nextSibling);
                }

            }).catch(() => {
                parent.insertBefore(currentPost, nextSibling);
            });
    }
});









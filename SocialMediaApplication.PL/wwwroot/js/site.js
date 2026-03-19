// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let btn = document.getElementById("post-text");
let modal = document.getElementById("modal-div");
let createPostForm = document.getElementById("form-container");
let likesContainer = document.getElementById("likes-container");

// (Show - Hide) Modal & Create-Post Form
modal.onclick = function () {
    if (createPostForm.classList.contains("show")) {
        $("#form-container").html("");
        createPostForm.classList.toggle("show");
    }
    if (likesContainer.classList.contains("show")) {
        $("#likes-container").html("");
        likesContainer.classList.toggle("show");
    }
    modal.classList.toggle("show");
};


// Get Create-Post Form From Server
$("#post-text").click(function () {
    modal.classList.add("show");
    createPostForm.classList.add("show");
    fetch("/Home/GetCreateForm",
        {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(
                "Create"
            )
        })
        .then(res => res.text())
        .then(result => {

            $("#form-container").html(result);
            loadFormEvents();
            bindCreatePostForm();
        });
});


// Load Create Post Form Events
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

// Create Post
function bindCreatePostForm(currentPost, postId) {
    $("#create-post").validate({

        submitHandler: function (form) {
            let formData = new FormData(form);
            if(postId)
                formData.append("postId", postId);
            fetch("/Home/AddOrUpdatePost", {
                method: "POST",
                body: formData
            })
                .then(res => res.text())
                .then(result => {
                    
                    if (result.includes("create-post")) {
                        $("#form-container").html(result);
                        loadFormEvents();
                        bindCreatePostForm();
                    } else if (currentPost && postId && postId !== null) {
                        console.log("2");
                        createPostForm.classList.toggle("show");
                        $("#form-container").html("");
                        modal.classList.toggle("show");
                        let temp = document.createElement("div");
                        temp.innerHTML = result;

                        let newElement = temp.firstElementChild;

                        currentPost.replaceWith(newElement);
                    }
                    else{
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


// Show Post Options, Update Post, Delete Post
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
    else if (e.target.classList.contains("Update-Post")) {
        let currentPost = document.getElementsByClassName(`post-${e.target.dataset.postId}`)[0];
        fetch("/Home/GetCreateForm",
            {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(
                    e.target.dataset.postId
                )
            })
            .then(res => res.text())
            .then(result => {
                modal.classList.toggle("show");
                console.log(result);
                createPostForm.classList.toggle("show");
                $("#form-container").html(result);
                loadFormEvents();
                bindCreatePostForm(currentPost, e.target.dataset.postId);
            });
    }
});


// Toggle Like
document.addEventListener("click", async function (e) {
    if (e.target.classList.contains("like-button")) {
        let buttonElement = e.target;
        let postId = buttonElement.dataset.postId;
        let formData = new FormData();
        formData.append("postId", postId);
        let response = await fetch("/Post/ToggleLike", {
            method: "POST",
            //headers: {
            //    "Content-Type": "application/json"
            //},
            body: formData
        });
        if (response.ok) {
            let result = await response.json();
            let likesanchor = document.getElementById(`likes-of-${postId}`);
            let currentNumber = parseInt(likesanchor.textContent);

            if (result.liked) {
                buttonElement.classList.add("color-blue");
                likesanchor.textContent = currentNumber + 1;
            } else {
                buttonElement.classList.remove("color-blue");
                likesanchor.textContent = currentNumber - 1;

            }
        }
    }
});

document.addEventListener("click", async function (e) {
    if (e.target.classList.contains("likes")) {
        let buttonElement = e.target;
        let postId = buttonElement.dataset.postId;
        let formData = new FormData();
        formData.append("postId", postId);
        let response = await fetch("/Post/GetPostLikes", {
            method: "POST",
            //headers: {
            //    "Content-Type": "application/json"
            //},
            body: formData
        });
        if (response.ok) {
            modal.classList.add("show");
            likesContainer.classList.add("show");
            let result = await response.text();
            console.log(result);
            $("#likes-container").html(result);
            //loadLikesDivEvents();
        }
        
    }
});















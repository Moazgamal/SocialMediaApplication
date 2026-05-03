// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let btn = document.getElementById("post-text");
let modal = document.getElementById("modal-div");
let createPostForm = document.getElementById("form-container");
let likesContainer = document.getElementById("likes-container");
let postWithCommentsContainer = document.getElementById("post-with-comments-container");
let allPostsDiv = document.getElementById("all-posts");
let noPostsDiv1 = document.getElementById("no-posts-div1");
let noPostsDiv2 = document.getElementById("no-posts-div2");


// (Show - Hide) Modal & Create-Post Form
document.addEventListener("click", function (e) {
    if (e.target.id=="modal-div") {

        if (postWithCommentsContainer.classList.contains("show")) {

            if (window.getComputedStyle(e.target).zIndex == "2") {
                //console.log("fasfasdfasdf");
                let postWithCommentsBody = document.getElementById("postWithCommentsBody");
                postWithCommentsBody.innerHTML = "";
                postWithCommentsContainer.classList.toggle("show");
                e.target.classList.toggle("show");
            } else {
                createPostForm.innerHTML = "";
                createPostForm.classList.toggle("show");
                e.target.classList.add("show");
                e.target.style.zIndex = "2";
            }
        }
        else if (createPostForm.classList.contains("show")) {
            $("#form-container").html("");
            createPostForm.classList.toggle("show");
            e.target.classList.toggle("show");
        }
        else if (likesContainer.classList.contains("show")) {
            $("#likes-container").html("");
            likesContainer.classList.toggle("show");
            e.target.classList.toggle("show");
        }
    }
});


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
        if (window.getComputedStyle(modal).zIndex == "2") {
            createPostForm.classList.toggle("show");
            $("#form-container").html("");
            modal.classList.toggle("show");
        }
        else {
            $("#form-container").html("");
            createPostForm.classList.toggle("show");
            modal.style.zIndex = "2";
        }
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
function bindCreatePostForm(currentPost, currentPostWithComments, postId) {
    let form = $("#create-post");

    form.removeData("validator");
    form.removeData("unobtrusiveValidation");
    form.validate({

        submitHandler: function (form) {
            let formData = new FormData(form);
            if(postId)
                formData.append("postId", postId);
            let submitBtn = $(form).find("button[type='submit']");
            submitBtn.prop("disabled", true); // 🔒 منع double submit

            fetch("/Home/AddOrUpdatePost", {
                method: "POST",
                body: formData
            })
                .then(res => res.text())
                .then(result => {
                    
                    if (result.includes("create-post")) {
                        $("#form-container").html(result);
                        loadFormEvents();
                        bindCreatePostForm(currentPost, currentPostWithComments, postId);
                    } else if (currentPost && postId && postId !== null) {

                        console.log("2");
                        createPostForm.classList.toggle("show");
                        $("#form-container").html("");
                        let temp = document.createElement("div");
                        temp.innerHTML = result;

                        let newElement = temp.firstElementChild;

                        let clonedElement = newElement.cloneNode(true);

                        if (currentPostWithComments) {
                            modal.style.zIndex = "2";

                            currentPostWithComments.replaceWith(clonedElement);

                        }
                        else {
                            modal.classList.toggle("show");
                        }
                        currentPost.replaceWith(newElement);
                    }
                    else{
                        createPostForm.classList.toggle("show");
                        $("#form-container").html("");
                        modal.classList.toggle("show");
                        if (document.getElementById("no-posts-div1") && window.getComputedStyle(noPostsDiv1).display === "flex") {
                            document.getElementById("no-posts-div1").style.display = "none";
                        }
                        if (document.getElementById("no-posts-div2")  && window.getComputedStyle(noPostsDiv2).display === "flex") {
                            document.getElementById("no-posts-div2").style.display = "none";
                        }
                       
                        $("#all-posts").prepend(result);
                    }
                   })
                    .catch(err => {
                        console.error(err);
                        alert("Something went wrong");
                    })
                    .finally(() => {
                        submitBtn.prop("disabled", false); // 🔓 رجّع الزرار
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
        let currentPost = allPostsDiv.getElementsByClassName(`post-${e.target.dataset.postId}`)[0];
        
        

        let postUserSettings = document.getElementById(`post-user-settings-${e.target.dataset.postId}`);
        let parent = currentPost.parentNode;
        let nextSibling = currentPost.nextSibling;

        currentPost.remove();
        
        if (allPostsDiv.children.length === 0) {
            document.getElementById("no-posts-div2").style.display = "flex";
        }
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
                if(result.success === true){
                    if(postWithCommentsContainer.classList.contains("show")){
                        modal.click();
                    }
                }
                if (result.success === false) {
                    if (allPostsDiv.children.length === 0)
                        document.getElementById("no-posts-div2").style.display = "none";
                    parent.insertBefore(currentPost, nextSibling);
                }

            }).catch(() => {
                if (allPostsDiv.children.length === 0)
                    document.getElementById("no-posts-div2").style.display = "none";

                parent.insertBefore(currentPost, nextSibling);
            });
    }
    else if (e.target.classList.contains("Update-Post")) {
        let currentPost = allPostsDiv.getElementsByClassName(`post-${e.target.dataset.postId}`)[0];
        let currentPostWithComments = postWithCommentsContainer.getElementsByClassName(`post-${e.target.dataset.postId}`)[0];
        console.log("dfsadfads");
        console.log(currentPostWithComments);
        
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
                
                if (postWithCommentsContainer.classList.contains("show")) {
                    modal.style.zIndex = "4";
                }
                else
                    modal.classList.toggle("show");
                console.log(result);
                createPostForm.classList.add("show");
                console.log("iiiiiiiiiiiii");
                createPostForm.innerHTML = result;
                loadFormEvents();
                bindCreatePostForm(currentPost, currentPostWithComments, e.target.dataset.postId);
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
            let currentPost = allPostsDiv.getElementsByClassName(`post-${postId}`)[0];
            let likesanchor = currentPost.querySelector(`#likes-of-${postId}`);
            let currentNumber = parseInt(likesanchor.textContent);

            if (postWithCommentsContainer.classList.contains("show")) {
                if(result.liked){
                    buttonElement.classList.add("color-blue");
                    likesanchor.textContent = currentNumber + 1;
                    let likesanchorForPostWithComments = postWithCommentsContainer.querySelector(`#likes-of-${postId}`);
                    likesanchorForPostWithComments.textContent = currentNumber + 1;
                    let likesButton = currentPost.querySelector(`#like-button-of-${postId}`);
                    likesButton.classList.add("color-blue");
                }
                else {
                    buttonElement.classList.remove("color-blue");
                    likesanchor.textContent = currentNumber - 1;
                    let likesanchorForPostWithComments = postWithCommentsContainer.querySelector(`#likes-of-${postId}`);
                    likesanchorForPostWithComments.textContent = currentNumber - 1;
                    let likesButton = currentPost.querySelector(`#like-button-of-${postId}`);
                    likesButton.classList.remove("color-blue");
                }
            }
            else {
                if(result.liked){
                    buttonElement.classList.add("color-blue");
                    likesanchor.textContent = currentNumber + 1;

                } else {
                    buttonElement.classList.remove("color-blue");
                    likesanchor.textContent = currentNumber - 1;
                }
            }
            
        }
    }
});

// Show Likes

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

// Show Post With Comments

document.addEventListener("click", async function (e) {
    if (e.target.classList.contains("comment-button")) {
        if(!postWithCommentsContainer.classList.contains("show")){
            modal.classList.toggle("show");

            postWithCommentsContainer.classList.add("show");

            let buttonElement = e.target;

            let postId = buttonElement.dataset.postId;

            let userName = buttonElement.dataset.userName;

            let postOwnerName = document.getElementById("post-owner-name");
            postOwnerName.innerText = userName + '\'s Post';

            let postResponse = await fetch(`/POST/GetPostWithComments?postId=${postId}`, {
                method: "GET",
            });

            let postHtml = await postResponse.text();
        
            //console.log(postHtml);
            let postWithCommentsBody = document.getElementById("postWithCommentsBody");
            postWithCommentsBody.innerHTML = postHtml;

            let closePostButton = document.getElementById("close-post-btn");
            closePostButton.dataset.postId = postId;

            ////let formData = new FormData();
            ////formData.append("postId", postId);
            //let header = document.createElement("div");
            //header.classList.add("post-with-comments-header");
            //let 
        }   
    }
});

// close post with comments 
document.addEventListener("click", async function (e) {
    let btn = e.target.closest("#close-post-btn");

    if (!btn) return;

    document.getElementById("postWithCommentsBody").innerHTML = "";
    postWithCommentsContainer.classList.remove("show");
    modal.classList.remove("show");
});


// ADD Comment 

document.getElementById("commentForm")
.addEventListener("submit", async function (e) {
    e.preventDefault();

    let form = this;
    let formData = new FormData(form);

    let btn = document.getElementById("add-comment-btn");
    btn.disabled = true;

    let currentPost = postWithCommentsContainer.querySelector("#post");
    let postId = currentPost.dataset.postId;

    formData.append("postId", postId);

    let response = await fetch("/Post/AddComment", {
        method: "POST",
        body: formData
    });

    if (response.ok) {

        let result = await response.text();

        document.getElementById("comments-container").insertAdjacentHTML("afterbegin", result);

        document.querySelector("[data-valmsg-for='Content']").innerText = "";

        let curHomePost = allPostsDiv.getElementsByClassName(`post-${postId}`)[0];

        let commentsCounterOfPost = curHomePost.querySelector(`#comments-of-${postId}`);

        let currentNumber = parseInt(commentsCounterOfPost.textContent);

        commentsCounterOfPost.textContent = currentNumber + 1;

        let commentsCounterOfPostWithComments = currentPost.querySelector(`#comments-of-${postId}`);

        commentsCounterOfPostWithComments.textContent = currentNumber + 1;

        form.reset();
    } else {
        let errorText = await response.text();
    
        document.querySelector("[data-valmsg-for='Content']").innerText = "Invalid comment";
        e.preventDefault();
    }
    btn.disabled = false;
});


// disable input comment 
let commentInput = document.getElementById("comment-input");
let addCommbtn = document.getElementById("add-comment-btn");

commentInput.addEventListener("input", () => {
    addCommbtn.disabled = commentInput.value.trim() === "";
});


// Toggle Comment Like

//let likeCommentBtn = document.getElementById("like-commment");
document.addEventListener("click", async function(e) {
    if (e.target.id === "like-comment") {
        //let commentId = parseInt(e.target.dataset.commentId);
        let commentId = e.target.dataset.commentId;
        let formData = new FormData();
        formData.append("commentId", commentId);

        let response = await fetch("/Post/ToggleCommentLike", {
            method: "POST",
            body: formData
        });

        if (response.ok) {
            let result = await response.json();
            let commentLikes = postWithCommentsContainer.querySelector(`#comment-likes-${commentId}`);
            console.log(commentLikes);
            let currentNumber = parseInt(commentLikes.textContent);
            if (result.liked) {
                e.target.classList.add("color-blue");
                commentLikes.textContent = currentNumber + 1;

            } else {
                e.target.classList.remove("color-blue");
                commentLikes.textContent = currentNumber - 1;
            }
        }

    }
});

// SHOW COMMENT OPTIONS

document.addEventListener("click", async function (e) {
    let commentOptionsBtn = e.target.closest(".comment-menu-btn");
    if (commentOptionsBtn) {

        let commentMenuContainer = commentOptionsBtn.closest(".comment-menu-container");
        let menu = commentMenuContainer.querySelector(".comment-menu");
        let commentId = commentMenuContainer.dataset.commentId;

        if (menu.classList.contains("show")) {
            menu.classList.remove("show");
            menu.innerHTML = "";
            return;
        }
        else {
            let response = await fetch(`/Post/GetPrivileges?commentId=${commentId}`);
            let data = await response.json();

            menu.innerHTML = "";
            if (data.isOwner == true) {
                console.log(data.IsOwner);
                menu.innerHTML += `<button class="edit-comment" data-comment-id="${commentId}">Edit</button>`;
                menu.innerHTML += `<button class="delete-comment" data-comment-id="${commentId}">Delete</button>`;
            }else {
                console.log(data.IsOwner);
                menu.innerHTML += `<button class="hide-comment" data-comment-id="${commentId}">Hide</button>`;
            }
            menu.classList.add("show");
        }
    }
});


// DELETE COMMENT

document.addEventListener("click", async function (e) {
    
    let deleteBtn = e.target.closest(".delete-comment");
    if (deleteBtn) {

        let commentId = deleteBtn.dataset.commentId;
        let response = await fetch("/Post/DeleteComment", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(commentId)
        });

        let result = await response.json();

        if (result.success) {
            let commentElement = deleteBtn.closest(".comment");
            commentElement.remove();

            let currentPost = postWithCommentsContainer.querySelector("#post");
            let postId = currentPost.dataset.postId;

            let curHomePost = allPostsDiv.getElementsByClassName(`post-${postId}`)[0];

            let commentsCounterOfPost = curHomePost.querySelector(`#comments-of-${postId}`);

            let currentNumber = parseInt(commentsCounterOfPost.textContent);

            commentsCounterOfPost.textContent = currentNumber - 1;

            let commentsCounterOfPostWithComments = currentPost.querySelector(`#comments-of-${postId}`);

            commentsCounterOfPostWithComments.textContent = currentNumber - 1;

        } else {
            console.log("Delete failed");
        }
    }
});



// UPDATE COMMENT

document.addEventListener("click", async function (e) {

    let editBtn = e.target.closest(".edit-comment");

    if (editBtn) {

        let commentId = editBtn.dataset.commentId;
        let commentElement = editBtn.closest(".comment");
        let contentEl = commentElement.querySelector("p");

        let oldContent = contentEl.textContent;

        contentEl.innerHTML = `
            <input type="text" class="edit-input" value="${oldContent}" />
            <button class="save-edit">Save</button>
            <button class="cancel-edit">Cancel</button>
        `;
    }
});

// save edit comment

document.addEventListener("click", async function (e) {

    let saveBtn = e.target.closest(".save-edit");

    if (saveBtn) {

        let commentElement = saveBtn.closest(".comment");
        let input = commentElement.querySelector(".edit-input");
        let newContent = input.value;

        let commentId = commentElement
            .querySelector(".comment-menu-container")
            .dataset.commentId;

        let formData = new FormData();
        formData.append("commentId", commentId);
        formData.append("content", newContent);

        let response = await fetch("/Post/UpdateComment", {
            method: "POST",
            body: formData
        });

        let result = await response.json();

        if (result.success) {
            // رجع النص
            let contentEl = commentElement.querySelector("p");
            contentEl.textContent = newContent;
        } else {
            console.log("Update failed");
        }
    }
});

// cancel edit comment
document.addEventListener("click", function (e) {

    let cancelBtn = e.target.closest(".cancel-edit");

    if (cancelBtn) {

        let commentElement = cancelBtn.closest(".comment");
        let input = commentElement.querySelector(".edit-input");

        let oldContent = input.defaultValue;

        let contentEl = commentElement.querySelector("p");
        contentEl.textContent = oldContent;
    }
});




















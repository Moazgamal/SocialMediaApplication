// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let btn = document.getElementById("post-text");
let modal = document.getElementById("modal-div");
//let createPostForm = document.getElementById("create-post");
let createPostForm = document.getElementById("form-container");
let closeButton = document.getElementById("close");

btn.onclick = function () {
    modal.classList.add("show");
    createPostForm.classList.add("show");
};
modal.onclick = function () {
    modal.classList.toggle("show");
    createPostForm.classList.toggle("show");
};
closeButton.onclick = function () {
    modal.classList.toggle("show");
    createPostForm.classList.toggle("show");
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

let postsContainer = document.getElementById("all-posts");
let form = $("#create-post");
form.validate({
    onkeyup: false
});

    form.on("submit", function (e) {
        //var creatPostForm = e.target;
        
        if (!form.valid()) {
            e.preventDefault();
            return; // فيه errors → سيبه يعرضهم وخلاص
        }
        e.preventDefault();
        var formData = new FormData(this);
        //var partialContent = postsContainer.innerHTML;
        var newDiv = document.createElement('div');
        fetch("/Home/AddPost", {
            method: "Post",
            body: formData
        })
            .then(response => {
                if (!response.ok) {
                    
                    document.getElementById("form-container").innerHTML = html;

                    // فعل الـ validation تانى
                    $.validator.unobtrusive.parse("#form-container");

                    return; // وقف هنا
                }

                return response.text();
            })
            .then(result => {
                newDiv.innerHTML = result;
                newDiv.id = "post";
                postsContainer.insertBefore(newDiv, postsContainer.firstChild);
                this.reset();
                form.valid();
            })
            .catch(err => console.log(err))
    });







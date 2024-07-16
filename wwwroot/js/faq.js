document.addEventListener("DOMContentLoaded", function () {
    const questions = document.querySelectorAll(".faq-question");

    questions.forEach(question => {
        question.addEventListener("click", function () {
            const answer = this.nextElementSibling;
            const isActive = this.classList.contains("active");

            questions.forEach(q => {
                q.classList.remove("active");
                q.nextElementSibling.classList.remove("show");
            });

            if (!isActive) {
                this.classList.add("active");
                answer.classList.add("show");
            }
        });
    });
});

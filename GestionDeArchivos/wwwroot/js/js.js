function mostrarContrasena() {
    var tipo = document.getElementById("pass");
    var msjPass = document.getElementById("msjPass");
    var iconPass = document.getElementById("iconPass");

    if (tipo.type == "password") {
        tipo.type = "text";
        iconPass.classList.replace("bi-eye-fill", "bi-eye-slash");
        msjPass.textContent = "Ocultar Contraseña"
    } else {
        tipo.type = "password";
        iconPass.classList.replace("bi-eye-slash", "bi-eye-fill");
        msjPass.textContent = "Mostrar Contraseña"
    }
}

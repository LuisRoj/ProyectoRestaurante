// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification

function confirmDelete(productName, productId, deleteUrl, imageUrl) {

    var maxLength = 50;
    document.getElementById("productDescription").textContent = `¿Deseas eliminar el producto "${productName}"?`;
    document.getElementById("productImage").src = imageUrl;
    document.getElementById("deleteButton").setAttribute("onclick", `deleteProduct('${deleteUrl}')`);
    document.getElementById("confirmModal").style.display = "block";
}

function deleteProduct(deleteUrl) {
 
    window.location.href = deleteUrl;
}

function closeModal() {
    document.getElementById("confirmModal").style.display = "none";
}


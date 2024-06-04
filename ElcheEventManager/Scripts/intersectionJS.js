
//console.log(eventId);

var map = L.map('map').fitWorld().setView([38.2654712, -0.6987196], 13);
L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

//Esri GeoCodeService to extract streets data.
var geocodeService = L.esri.Geocoding.geocodeService();
//console.log(geocodeService);

var token = "AAPKd7d5597d167f44c5abd032d8768b5f14Ipq2J5HEEsbiNITk91uHG5Xkzi3I8bVWxvzasucZfN6hPafqs5-uuvl_HnnTFPlr";

map.on('click', function (e) {

    map.closePopup();

    var intersectionData;
    geocodeService.reverse().latlng(e.latlng).intersection(true).token(token).run(function (error, result) {

        console.log(e.latlng);
        console.log(result)
        if (error) {
            console.error("Error:", error);
            return;
        }

        if (result && result.address) {
            streets = (result.address.ShortLabel).split('& ');
            intersectionData = {
                streetsName: streets,
                latitude: e.latlng.lat,
                longitude: e.latlng.lng
            }

            console.log(streets);
            CallModalCreate(intersectionData);
            //console.log(streets);
            //L.marker(e.latlng).addTo(map).bindPopup(result.address.ShortLabel).openPopup();
        } else {
            console.log("Dirrección no encontrada para el punto seleccionado.");
        }

    });

});

$(document).ready(function () {
    GetIntersections();
});


//function GetIntersections() {
//    $.ajax({
//        url: '../GetIntersections/1',
//        type: 'get',
//        datatype: 'json',
//        contentType: 'application/json',
//        success: function (response) {
//            if (response == null || response == undefined || response.length == 0) {
//                alert('El mapa está vacio actualmente.')
//            }
//            else {
//                $.each(response.intersections, function (index, item) {
//                    //console.log(item)       //response.intersections[index]
//                    //usar item.id para darles a los botones de edit y borrar
//                    var popupContent = createPopupContent(item);

//                    L.marker([item.longitude, item.latitude]).addTo(map).bindPopup(popupContent);
//                });
//            }

//        },
//        error: function (er) {
//            alert("Error al cargar los datos de las intersecciones.")
//        }
//    });
//}

var intersectionsCount;
function GetIntersections() {
    fetch(`/Intersections/GetIntersections/${eventId}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Ha habido un error al cargar las intersecciones.');
            }
            return response.json();
        })
        .then(response => {
            if (!response || response.length === 0) {
                alert('El mapa está vacio actualmente.');
            } else {
                intersectionsCount = response.intersections.length;
                console.log(`Number of intersections: ${intersectionsCount}`);

                response.intersections.forEach(item => {

                    const popupContent = createPopupContent(item);

                    L.marker([item.latitude, item.longitude]).addTo(map).bindPopup(popupContent);
                });
            }
        })
        .catch(error => {
            console.error('Error al cargar los datos de las intersecciones:', error);
        });
}

function getAntiForgeryToken() {
    var token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    //console.log(token)
    return token;
}


function HideModal() {
    $('#createIntersectionModal').modal('hide');
    $('#editIntersectionModal').modal('hide');
}

function ClearData() {

}

function CallModalCreate(intersectionData) {
    $.ajax({
        url: '/Intersections/Create',
        dataType: 'html',
        type: 'get',
        success: function (data) {
            $('#myPartialContainer').html(data);

            fillCreateModal(intersectionData);

            $('#createIntersectionModal').modal('show');
        },
        error: function (er) {
            alert("No se pudo cargar el punto seleccionado.");
        }
    });
}

function fillCreateModal(intersectionData) {
    //$('#createIntersectionModal #primaryStreetInput').val(intersectionData.streetsName[0]);
    //$('#createIntersectionModal #secondaryStreetInput').val(intersectionData.streetsName[1]);
    var primaryStreetDropdown = $('#createIntersectionModal #primaryStreetInput');
    var secondaryStreetDropdown = $('#createIntersectionModal #secondaryStreetInput');
    primaryStreetDropdown.empty();
    secondaryStreetDropdown.empty();

    $.each(intersectionData.streetsName, function (index, street) {
        primaryStreetDropdown.append($('<option>', {
            value: street,
            text: street
        }));
        secondaryStreetDropdown.append($('<option>', {
            value: street,
            text: street
        }));
    });

    if (intersectionData.streetsName.length > 1) {
        secondaryStreetDropdown.val(intersectionData.streetsName[1]);
    }
    // Aqui le relleno el html con el valor de la latitud y longitud. Depende si este valor se pasa como decimal o como string
    // al html
    $('#createIntersectionModal #latitudeInput').val(intersectionData.latitude);
    $('#createIntersectionModal #longitudeInput').val(intersectionData.longitude);
    $('#createIntersectionModal #eventIdInput').val(eventId);
    $('#createIntersectionModal #quantityInput').val(1)
}
//function CallModalCreate(intersectionData) {
//    fetch('/Intersections/Create', {
//        method: 'GET',
//        headers: {
//            'Content-Type': 'text/html'
//        }
//    })
//        .then(response => {
//            if (!response.ok) {
//                throw new Error("Network response was not ok");
//            }
//            return response.text();
//        })
//        .then(data => {
//            document.querySelector('#myPartialContainer').innerHTML = data;

//            var primaryStreetDropdown = document.querySelector('#createIntersectionModal #primaryStreetInput');
//            var secondaryStreetDropdown = document.querySelector('#createIntersectionModal #secondaryStreetInput');
//            primaryStreetDropdown.innerHTML = '';
//            secondaryStreetDropdown.innerHTML = '';

//            intersectionData.streetsName.forEach(street => {
//                let primaryOption = new Option(street, street);
//                let secondaryOption = new Option(street, street);
//                primaryStreetDropdown.appendChild(primaryOption);
//                secondaryStreetDropdown.appendChild(secondaryOption);
//            });

//            if (intersectionData.streetsName.length > 1) {
//                secondaryStreetDropdown.value = intersectionData.streetsName[1];
//            }

//            document.querySelector('#createIntersectionModal #latitudeInput').value = intersectionData.latitude;
//            document.querySelector('#createIntersectionModal #longitudeInput').value = intersectionData.longitude;
//            document.querySelector('#createIntersectionModal #eventIdInput').value = eventId;

//            $('#createIntersectionModal').modal('show');
//        })
//        .catch(error => {
//            alert("No se pudo cargar el punto seleccionado.");
//            console.error("There was a problem with the fetch operation:", error);
//        });
//}

function CallModalEdit(id) {
    fetch(`/Intersections/Edit/${id}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'html',
            'RequestVerificationToken': getAntiForgeryToken()
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Error al cargar la pantalla de edición.');
            }
            return response.text();

        })
        .then(data => {
            document.getElementById('myPartialContainer').innerHTML = data;
            $('#editIntersectionModal').modal('show');
        })
        .catch(error => {
            console.error('Error:', error);
            alert('No se ha podido acceder a los datos del punto seleccionado.');
        });
}

function deleteIntersection(item) {
    var marker = L.marker([item.latitude, item.longitude]);
    console.log(marker);

    if (confirm('Estás seguro de eliminar el punto actual?')) {
        fetch(`/Intersections/Delete/${item.id}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify({ id: item.id })
        })
            .then(response => {
                if (response.ok) {
                    // Redirect to the Index page after successful deletion
                    if (!map.hasLayer(marker)) {
                        console.log("No encuentro el marker")
                    }
                    console.log(marker);
                    map.removeLayer(marker);

                    window.location.reload();
                    //GetIntersections();
                } else {
                    return response.text().then(text => { throw new Error(text); });
                }
            })
            .catch(error => {
                console.error('Error:', error);
                alert('Ha ocurrido un error mientras se eliminaba el punto.');
            });
    }
}

//function createPopupContent(item) {
//    var popupContent = document.createElement("div");

//    var antiForgeryTokenInput = document.querySelector('input[name="__RequestVerificationToken"]').value;
//    //var antiForgeryTokenInput = document.createElement("input");
//    //antiForgeryTokenInput.type = "hidden";
//    //antiForgeryTokenInput.name = "__RequestVerificationToken";
//    //antiForgeryTokenInput.value = document.querySelector('input[name="__RequestVerificationToken"]').value;
//    //popupContent.appendChild(antiForgeryTokenInput);

//    var editButton = document.createElement("button");
//    editButton.textContent = "Editar";
//    editButton.dataset.id = item.id;
//    editButton.className = "btn btn-primary btn-sm";
//    editButton.onclick = function () {
//        CallModalEdit(item.id);
//    }
//    popupContent.appendChild(editButton);

//    var deleteButton = document.createElement("button");
//    deleteButton.textContent = "Eliminar";
//    deleteButton.dataset.id = item.id;
//    deleteButton.className = "btn btn-danger btn-sm";
//    deleteButton.onclick = function () {
//        deleteIntersection(item);
//    };
//    popupContent.appendChild(deleteButton);

//    return popupContent;
//}
function createPopupContent(item) {
    var popupContent = document.createElement("div");

    var primaryStreetLabel = document.createElement("label");
    primaryStreetLabel.textContent = "Calle Principal: " + item.primary_street;
    primaryStreetLabel.style.marginBottom = "1rem"; 
    popupContent.appendChild(primaryStreetLabel);

    var secondaryStreetLabel = document.createElement("label");
    secondaryStreetLabel.textContent = "Calle Secundaria: " + item.secondary_street;
    secondaryStreetLabel.style.marginBottom = "1rem"; 
    popupContent.appendChild(secondaryStreetLabel);
    popupContent.appendChild(document.createElement("br"));

    var quantityLabel = document.createElement("label");
    quantityLabel.textContent = "Cantidad: " + item.quantity;
    quantityLabel.style.marginBottom = "1rem"; 
    quantityLabel.style.marginRight = "5rem"
    popupContent.appendChild(quantityLabel);

    var editButton = document.createElement("button");
    editButton.textContent = "Editar";
    editButton.dataset.id = item.id;
    editButton.className = "btn btn-outline-primary btn-sm";
    editButton.style.marginRight = "0.5rem"; 
    editButton.onclick = function () {
        CallModalEdit(item.id);
    }
    popupContent.appendChild(editButton);

    var deleteButton = document.createElement("button");
    deleteButton.textContent = "Eliminar";
    deleteButton.dataset.id = item.id;
    deleteButton.className = "btn btn-outline-danger btn-sm";
    deleteButton.onclick = function () {
        deleteIntersection(item);
    };
    popupContent.appendChild(deleteButton);

    return popupContent;
}

var customControl = L.Control.extend({
    options: {
        position: 'topright' 
    },

    onAdd: function (map) {
        var container = L.DomUtil.create('div', 'custom-control');

        var button1 = L.DomUtil.create('button', '', container);
        button1.innerHTML = 'Eventos';
        button1.style.margin = '0.5rem';
        button1.onclick = function () {
            window.location.href = '/Events'; 
        };

        var button2 = L.DomUtil.create('button', '', container);
        button2.innerHTML = 'Listado';
        button2.style.margin = '0.5rem';
        button2.onclick = function () {
            window.location.href = `/Intersections/FencesList/${eventId}`; 
        };

        // Avoid click in map
        L.DomEvent.disableClickPropagation(container);

        return container;
    }
});

// Añadir el control personalizado al mapa
map.addControl(new customControl());
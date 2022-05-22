$(document).ready(function () {
    setInterval(function () {
        AtualizaGraficos()
    }, 10000)
    window.initMap = initMap;
})

function AtualizaGraficos() {
    AtualizaGraficoNivelHoraAHoraDispositivo();
    AtualizaGraficoNivelDiaADiaDispositivo();
    AtualizaGraficoValorDeChuvaHoraAHoraDispositivo();
    AtualizaGraficoValorDeChuvaDiaADiaDispositivo();
}
function AtualizaGraficoNivelHoraAHoraDispositivo() {
    var dispositivoSelecionado = $('#dispositivoReferencia').val();

    $.ajax({
        url: "/api/GetWaterLevelFromLastDayMeasuresFromDispositivo",
        data: { dispositivoId: dispositivoSelecionado },
        success: function (dados) {

            var dataPoints = [];


            var result = dados;

            for (var i = 0; i < result.length; i++) {
                dataPoints.push({ label: result[i].label, y: result[i].y });
            }

            var chart = new CanvasJS.Chart("hourlyWaterLevelDeviceChart", {
                backgroundColor: "#ffffff",
                theme: "light2",
                animationEnabled: false,
                zoomEnabled: false,

                title: {
                    text: "Valor de Nível d'Água Medido Hora a Hora",
                    fontColor: "#000000",
                    fontSize: 25,
                    padding: 10,
                    margin: 15,
                    fontWeight: "bold"
                },
                axisY: {
                    title: "Valor de Nível",
                    suffix: "mm",


                },
                axisX: {
                    title: "Horário",
                    suffix: "h",

                },
                data: [
                    {
                        type: "splineArea",
                        markerSize: 7,
                        color: "#2196f3",
                        dataPoints: dataPoints,
                    }
                ]
            });
            chart.render();
        }
    })
}

function AtualizaGraficoNivelDiaADiaDispositivo() {
    var dispositivoSelecionado = $('#dispositivoReferencia').val();

    $.ajax({
        url: "/api/GetWaterLevelFromLastMonthMeasuresFromDispositivo",
        data: { dispositivoId: dispositivoSelecionado },
        success: function (dados) {

            var dataPoints = [];

            var result = dados;

            for (var i = 0; i < result.length; i++) {
                dataPoints.push({ label: result[i].label, y: result[i].y });
            }

            var chart = new CanvasJS.Chart("dailyWaterLevelDeviceChart", {
                backgroundColor: "#ffffff",
                theme: "light2",
                animationEnabled: false,
                zoomEnabled: false,

                title: {
                    text: "Valor de Nível d'Água Medido Dia a Dia",
                    fontColor: "#000000",
                    fontSize: 25,
                    padding: 10,
                    margin: 15,
                    fontWeight: "bold"
                },
                axisY: {
                    title: "Valor de Nível",
                    suffix: "mm",
                },
                axisX: {
                    title: "Dia do Mês",
                },
                data: [
                    {
                        type: "splineArea",
                        markerSize: 7,
                        color: "#1976d2",
                        dataPoints: dataPoints,
                    }
                ]
            });
            chart.render();
        }
    })
}

function AtualizaGraficoValorDeChuvaHoraAHoraDispositivo() {
    var dispositivoSelecionado = $('#dispositivoReferencia').val();

    $.ajax({
        url: "/api/GetRainValueFromLastDayMeasuresFromDispositivo",
        data: { dispositivoId: dispositivoSelecionado },
        success: function (dados) {

            var dataPoints = [];

            var result = dados;

            for (var i = 0; i < result.length; i++) {
                dataPoints.push({ label: result[i].label, y: result[i].y });
            }

            var chart = new CanvasJS.Chart("hourlyRainValueDeviceChart", {
                backgroundColor: "#ffffff",
                theme: "light2",
                animationEnabled: false,
                zoomEnabled: false,

                title: {
                    text: "Valor de Qtd. Chuva Medido Hora a Hora",
                    fontColor: "#000000",
                    fontSize: 25,
                    padding: 10,
                    margin: 15,
                    fontWeight: "bold"
                },
                axisY: {
                    title: "Valor de Nível",
                    suffix: "mm",
                },
                axisX: {
                    title: "Horário",
                    suffix: "h",
                },
                data: [
                    {
                        type: "splineArea",
                        markerSize: 7,
                        color: "#2196f3",
                        dataPoints: dataPoints,
                    }
                ]
            });
            chart.render();
        }
    })
}

function AtualizaGraficoValorDeChuvaDiaADiaDispositivo() {
    var dispositivoSelecionado = $('#dispositivoReferencia').val();

    $.ajax({
        url: "/api/GetRainValueFromLastMonthMeasuresFromDispositivo",
        data: { dispositivoId: dispositivoSelecionado },
        success: function (dados) {

            var dataPoints = [];

            var result = dados;

            for (var i = 0; i < result.length; i++) {
                dataPoints.push({ label: result[i].label, y: result[i].y });
            }

            var chart = new CanvasJS.Chart("dailyRainValueDeviceChart", {
                backgroundColor: "#ffffff",
                theme: "light2",
                animationEnabled: false,
                zoomEnabled: false,

                title: {
                    text: "Valor de Qtd. Chuva Medido Dia a Dia",
                    fontColor: "#000000",
                    fontSize: 25,
                    padding: 10,
                    margin: 15,
                    fontWeight: "bold"
                },
                axisY: {
                    title: "Valor de Nível",
                    suffix: "mm",
                },
                axisX: {
                    title: "Dia do Mês",
                },
                data: [
                    {
                        type: "splineArea",
                        markerSize: 7,
                        color: "#1976d2",
                        dataPoints: dataPoints,
                    }
                ]
            });
            chart.render();
        }
    })
}

function initMap() {
    $.ajax({
        url: "/api/GetMapData",
        success: function (dados) {
            const map = new google.maps.Map(document.getElementById("map"), {
                zoom: 4,
                center: { lat: -14.235004, lng: -51.92528 },
                mapTypeId: "terrain",
            });

            if (dados != undefined) {
                for (const bairro in dados) {
                    // Add the circle for this city to the map.
                    const bairroCircle = new google.maps.Circle({
                        strokeColor: "#FF0000",
                        strokeOpacity: 0.8,
                        strokeWeight: 2,
                        fillColor: "#FF0000",
                        fillOpacity: 0.35,
                        map,
                        center: dados[bairro].center,
                        radius: Math.sqrt(dados[bairro].valorNivel) * 1000,
                    });

                    const marker = new google.maps.Marker({
                        position: dados[bairro].center,
                        map,
                        title: dados[bairro].descricao,
                    });

                    const infowindow = new google.maps.InfoWindow({
                        content: `<h1><strong>${dados[bairro].descricao}</strong></h1>Nivel Atual: ${dados[bairro].valorNivel.toFixed(3)} mm <br/> Qtd.Chuva: ${dados[bairro].valorChuva.toFixed(3)} mm`,
                    });

                    marker.addListener("click", () => {
                        infowindow.open({
                            anchor: marker,
                            map,
                            shouldFocus: false,
                        });
                    });
                }
            }
        }
    })

}
#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <EEPROM.h>

#include <WiFi.h>
#include <PubSubClient.h>

/* Configuração do OLED */
#define SCREEN_WIDTH 128 /* OLED display width, in pixels */
#define SCREEN_HEIGHT 64 /* OLED display height, in pixels */

/* Configuração do MQTT */
#define TOPICO_PUBLISH_NIVEL   "/iot/station001/attrs/ValorNivel"    /* tópico MQTT de envio de informações para Broker */
#define TOPICO_PUBLISH_CHUVA   "/iot/station001/attrs/ValorChuva"
#define ID_MQTT  "station001"     /* id mqtt (para identificação de sessão) */

/* Declaration for an SSD1306 display connected to I2C (SDA, SCL pins) */
Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, -1);

/* Definição dos pinos das teclas */
#define PINO_ESQUERDA   34
#define PINO_DIREITA    25
#define PINO_SUPERIOR   35
#define PINO_INFERIOR   33
#define PINO_ENTER      32

#define LED_D2  2

#define DIREITA 1
#define ESQUERDA 2
#define ENTER 3
#define INFERIOR 4
#define SUPERIOR 5

/* Bytes que serão salvos na E2PROM */
#define BYTES_E2PROM 100

/* mnemônicos dos menus */
char* Menu_Principal[] = { "Wi-Fi", "MQTT", "Volta" };
char* Menu_WIFI[] = { "SSID", "Senha", "Volta" };
char* Menu_MQTT[] = { "ID DISPOSITIVO", "IP SERVIDOR", "Volta" };

/* variáveis que vão para a E2PROM */
char  SSID_ID[20] = "aaaaaaaaaaaaaaaaaaa";
char  SSID_PASSWORD[20] = "aaaaaaaaaaaaaaaaaaa";
char  MQTT_ID[20] = "aaaaaaaaaaaaaaaaaaa";
char  MQTT_IP[20] = "1111111111111111111";

/* Configuração do MQTT */
const char* SSID = "HELIX_POSEIDON"; // SSID / nome da rede WI-FI que deseja se conectar
const char* PASSWORD = "Pax2019+"; // Senha da rede WI-FI que deseja se conectar
  
const char* BROKER_MQTT = "35.198.25.224"; //URL do broker MQTT que se deseja utilizar
int BROKER_PORT = 1883; // Porta do Broker MQTT
 
/* Variáveis e objetos globais */
WiFiClient espClient; // Cria o objeto espClient
PubSubClient MQTT(espClient); // Instancia o Cliente MQTT passando o objeto espClient
char EstadoSaida = '1';  //variável que armazena o estado atual da saída

float a = 0;
float voltage = 0;
float nivel = 0;

/* variáveis de swap para guarda a conversão do float para ascii */
char msgBuffer[5];
char msgBuffer1[5];
  
/* Prototypes */
void initSerial();
void initWiFi();
void initMQTT();
void reconectWiFi(); 
void mqtt_callback(char* topic, byte* payload, unsigned int length);
void VerificaConexoesWiFIEMQTT(void);
void InitOutput(void);

void setup()
{
  /* Serial.begin(9600); */
  if(!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) 
  { /* Address 0x3D for 128x64 */
    Serial.println(F("SSD1306 allocation failed"));
    for(;;);
  }
  
  display.clearDisplay();
  display.setTextSize(1);
  display.setTextColor(WHITE);

  pinMode(LED_D2, OUTPUT);
  pinMode(PINO_ESQUERDA, INPUT);
  pinMode(PINO_SUPERIOR, INPUT);
  pinMode(PINO_ENTER, INPUT);
  pinMode(PINO_INFERIOR, INPUT);
  pinMode(PINO_DIREITA, INPUT);

  EEPROM.begin(BYTES_E2PROM);
  LeE2PROM((char*)&SSID_ID, BYTES_E2PROM);
  delay(200);

  initWiFi();
  initMQTT();
}

void loop() 
{
  VerificaConexoesWiFIEMQTT(); /* garante funcionamento das conexões WiFi e ao broker MQTT */
  
  /* envia o status de todos os outputs para o Broker no protocolo esperado
  EnviaEstadoOutputMQTT(); */
  
  Serial.println(voltage);
  MQTT.publish(TOPICO_PUBLISH_NIVEL, dtostrf(nivel, 5, 0, msgBuffer1));
  MQTT.publish(TOPICO_PUBLISH_CHUVA, dtostrf(voltage, 5, 1, msgBuffer));
  
  MQTT.loop(); /* keep-alive da comunicação com broker MQTT */
  voltage += 0.5;
  nivel++;
  
  delay(500);
  
  if (QualTecla() == ESQUERDA) /* "senha" para entrar no menu */
  {
    while(QualTecla() == ESQUERDA);
    while(QualTecla() == 0);
    if (QualTecla() == ENTER)
    {
      while(QualTecla() == ENTER);
      while(QualTecla() == 0);
      ConfigMode();
    }
  }
  display.clearDisplay();
  display.display();
}

void ConfigMode()
{
  char opcao[3] = {0, 0, 0};
  
  do
  {
    opcao[0] = Menu(Menu_Principal, 3, (char*)&opcao[0]);
    if (opcao[0] == 2)
      break;
    
    if (opcao[0] == 0) /* Wi-Fi */
    {
      do
      {
        opcao[1] = Menu(Menu_WIFI, 3, (char*)&opcao[1]);
        
        if (opcao[1] == 2) /* Volta */
          break;
        if (opcao[1] == 0) /* SSID */
        {
          EntradaTexto((char*)&SSID_ID);
        }
        else
        if (opcao[1] == 1) /* Senha */
        {
          EntradaTexto((char*)&SSID_PASSWORD);
        }
      }
      while (1);
    }
    else
    if (opcao[0] == 1) /* MQTT */
    {
      do
      {
        opcao[1] = Menu(Menu_MQTT, 3, (char*)&opcao[1]);

        if (opcao[1] == 2) /* Volta */
          break;
        if (opcao[1] == 0) /* ID DISPOSITIVO */
        {
          EntradaTexto((char*)&MQTT_ID);
        }
        else
        if (opcao[1] == 1) /* IP SERVIDOR */
        {
          EntradaTexto((char*)&MQTT_IP);
        }
      }
      while (1);
    }
  }
  while (1);

  GravaE2PROM((char*)&SSID_ID, BYTES_E2PROM);
}

char Menu(char* texto[], char items, char* opcao_inicial)
{
  char opcao = *opcao_inicial;
  char tecla = 0;

  while (1)
  {
    tecla = TeclaPressionada();
    if (tecla == INFERIOR)
    {
      opcao++;
      if (opcao == items)
        opcao = 0;
    }
    else
    if (tecla == SUPERIOR)
    {
      opcao--;
      if (opcao == 255)
        opcao = items - 1;
    }
    else
    if (tecla == ENTER)
      return ( opcao );

    display.setCursor(0, 20);
    display.clearDisplay();
    display.print(MontaString(texto[opcao]));
    display.display();
  }
}

String MontaString(char* texto)
{
  String finalStr = "";
  while(*texto)
    {
    finalStr += *texto;
    texto++;
    }
  return finalStr;
}

char TeclaPressionada()
{
  unsigned int contador = 0;
  char QTecla;
  if (QTecla = QualTecla())
    while (QualTecla())
      contador++;
  
  if (contador > 10)
    return ( QTecla );
  else
    return ( 0 );
}

char QualTecla()
{
  char AuxC = 1;
  char tecla = 0;
  tecla = (!digitalRead(PINO_DIREITA)) |
          (!digitalRead(PINO_ESQUERDA) << 1) |
          (!digitalRead(PINO_ENTER) << 2) |
          (!digitalRead(PINO_INFERIOR) << 3) |
          (!digitalRead(PINO_SUPERIOR) << 4);
  
  for (unsigned char i = 1; i < 128; i <<= 1, AuxC++)
    if (tecla & i)
      return ( AuxC );
  return ( 0 ); /* nenhuma tecla pressionada */
}

void EntradaTexto(char* texto)
{
  char indice = 0;
  char tecla = 0;
  char indicador[20] = "*                  ";

  char pisca = 0;

  while(1)
  {
    tecla = TeclaPressionada();
    if (tecla == INFERIOR)
    {
      *(texto+indice) = *(texto+indice) + 1;
    }
    else
    if (tecla == SUPERIOR)
    {
      *(texto+indice) = *(texto+indice) - 1;
    }
    else
    if (tecla == ESQUERDA) /* deleta */
    {
      *(texto+indice) = ' ';
      indicador[indice] = ' ';
      indice--;
      indicador[indice] = '*';
      if (indice == 255)
        indice = 0;
    }
    else
    if (tecla == ENTER)
    {
      indicador[indice] = ' ';
      indice++;
      indicador[indice] = '*';
    }
    else
    if (tecla == DIREITA)
    {
      return; /* sai da função */
    }

    display.setCursor(0, 20);
    display.clearDisplay();
    display.println(MontaString(texto));
    if (pisca & 1)
      display.println(MontaString(indicador));
    display.display();

    pisca++;
  }
}

void GravaE2PROM(char* endereco, int num_bytes)
{
  char* pc = endereco;
  int i = 0;
  for (; i < num_bytes; i++, pc++)
    EEPROM.write(i, *pc); /* para o ESP32 equivale ao update */
}

void LeE2PROM(char* endereco, int num_bytes)
{
  char* pc = endereco;
  int i = 0;
  for (; i < num_bytes; i++, pc++)
    *pc = EEPROM.read(i);
}

//Função: inicializa comunicação serial com baudrate 9600 (para fins de monitorar no terminal serial 
//        o que está acontecendo.
//Parâmetros: nenhum
//Retorno: nenhum
void initSerial() 
{
    Serial.begin(9600);
}
 
//Função: inicializa e conecta-se na rede WI-FI desejada
//Parâmetros: nenhum
//Retorno: nenhum
void initWiFi() 
{
    delay(10);
    Serial.println("------Conexao WI-FI------");
    Serial.print("Conectando-se na rede: ");
    Serial.println(SSID);
    Serial.println("Aguarde");
     
    reconectWiFi();
}
  
//Função: inicializa parâmetros de conexão MQTT(endereço do 
//        broker, porta e seta função de callback)
//Parâmetros: nenhum
//Retorno: nenhum
void initMQTT() 
{
    MQTT.setServer(BROKER_MQTT, BROKER_PORT);   //informa qual broker e porta deve ser conectado
    MQTT.setCallback(mqtt_callback);            //atribui função de callback (função chamada quando qualquer informação de um dos tópicos subescritos chega)
}
  
//Função: função de callback 
//        esta função é chamada toda vez que uma informação de 
//        um dos tópicos subescritos chega)
//Parâmetros: nenhum
//Retorno: nenhum
void mqtt_callback(char* topic, byte* payload, unsigned int length) 
{
    String msg;
 
    //obtem a string do payload recebido
    for(int i = 0; i < length; i++) 
    {
       char c = (char)payload[i];
       msg += c;
    }
   
    //toma ação dependendo da string recebida:
    //verifica se deve colocar nivel alto de tensão na saída D0:
    //IMPORTANTE: o Led já contido na placa é acionado com lógica invertida (ou seja,
    //enviar HIGH para o output faz o Led apagar / enviar LOW faz o Led acender)
    if (msg.equals("lamp003@on|"))
    {
        digitalWrite(LED_D2, LOW);
        EstadoSaida = '0';
    }
 
    //verifica se deve colocar nivel alto de tensão na saída D0:
    if (msg.equals("lamp003@off|"))
    {
        digitalWrite(LED_D2, HIGH);
        EstadoSaida = '1';
    }
}
  
//Função: reconecta-se ao broker MQTT (caso ainda não esteja conectado ou em caso de a conexão cair)
//        em caso de sucesso na conexão ou reconexão, o subscribe dos tópicos é refeito.
//Parâmetros: nenhum
//Retorno: nenhum
void reconnectMQTT() 
{
    while (!MQTT.connected()) 
    {
        Serial.print("* Tentando se conectar ao Broker MQTT: ");
        Serial.println(BROKER_MQTT);
        if (MQTT.connect(ID_MQTT)) 
        {
            Serial.println("Conectado com sucesso ao broker MQTT!");
            //MQTT.subscribe(TOPICO_SUBSCRIBE); 
        } 
        else
        {
            Serial.println("Falha ao reconectar no broker.");
            Serial.println("Havera nova tentatica de conexao em 2s");
            delay(2000);
        }
    }
}
  
//Função: reconecta-se ao WiFi
//Parâmetros: nenhum
//Retorno: nenhum
void reconectWiFi() 
{
    //se já está conectado a rede WI-FI, nada é feito. 
    //Caso contrário, são efetuadas tentativas de conexão
    if (WiFi.status() == WL_CONNECTED)
        return;
         
    WiFi.begin(SSID, PASSWORD); // Conecta na rede WI-FI
     
    while (WiFi.status() != WL_CONNECTED) 
    {
        delay(100);
        Serial.print(".");
    }
   
    Serial.println();
    Serial.print("Conectado com sucesso na rede ");
    Serial.print(SSID);
    Serial.println("IP obtido: ");
    Serial.println(WiFi.localIP());
}
 
//Função: verifica o estado das conexões WiFI e ao broker MQTT. 
//        Em caso de desconexão (qualquer uma das duas), a conexão
//        é refeita.
//Parâmetros: nenhum
//Retorno: nenhum
void VerificaConexoesWiFIEMQTT(void)
{
    if (!MQTT.connected()) 
        reconnectMQTT(); //se não há conexão com o Broker, a conexão é refeita
     
     reconectWiFi(); //se não há conexão com o WiFI, a conexão é refeita
}
 
//Função: envia ao Broker o estado atual do output 
//Parâmetros: nenhum
//Retorno: nenhum
void EnviaEstadoOutputMQTT(void)
{
  /*
    if (EstadoSaida == '0')
      MQTT.publish(TOPICO_PUBLISH, "s|on");
 
    if (EstadoSaida == '1')
      MQTT.publish(TOPICO_PUBLISH, "s|off");
 */
    Serial.println("- Estado do LED onboard enviado ao broker!");
    delay(1000);
}
 
//Função: inicializa o output em nível lógico baixo
//Retorno: nenhum
void InitOutput(void)
{
    //IMPORTANTE: o Led já contido na placa é acionado com lógica invertida (ou seja,
    //enviar HIGH para o output faz o Led apagar / enviar LOW faz o Led acender)
    pinMode(LED_D2, OUTPUT);
    digitalWrite(LED_D2, HIGH);          
}

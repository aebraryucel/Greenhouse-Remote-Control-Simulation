import socket
import serial
import threading


ser = serial.Serial('COM1', baudrate = 9600, timeout=1)  # COM1 portundan 9600 bandında seri haberleşme başlat
serial.PARITY_NONE
serial.EIGHTBITS
serial.STOPBITS_ONE

s = socket.socket()                                                  # socket haberleşme kütüphanesini import et
host = "25.51.82.88"                                                # Sunucunun ip adresi
port = 7777                                                         # Sunucunun port numarası

id="sera1"


def threaded_client(socket):

    while True:

        try:

            request = s.recv(1024)  # Sunucudan gönderilen veri varsa al
            getData = request.decode('utf-8')  # Alınan veriyi okunabilir hale getir
            if getData != "":# Veri boş değilse
                print('Sunucunun talebi: ' + getData)
                ser.write(getData.encode())  # Arduino'ya sunucudan gelen veriyi yolla

        except socket.error as e:  # Soket haberleşmede herhangi bir sorun çıkarsa
            print("[Server aktif değil.] Mesaj:", e)





if __name__ == '__main__':

    try:
        s.connect((host, port))  # Verilen host ve port numarasındaki sokete bağlan
        print('Sunucuya bağlanıldı.')  # Ekrana bilgi yazdır
        identifier=id.encode('utf-8')
        s.send(identifier)

        t1 = threading.Thread(target=threaded_client, args=[s])
        t1.start()


        while True:  # sonsuz döngüye gir
            arduinoData = ser.readline().decode()  # Arduino'dan gelen veriyi al
            arduinoData = arduinoData.strip()
            if arduinoData == "":  # Eğer veri boşsa
                print("Bir veri yok")  # Ekrana bilgi yazdır
                continue  # while döngüsünün başına dön
            elif arduinoData =="0.00":
                print('Arduino Verisi: ' + arduinoData)
                data="99"
                s.send(data.encode('utf-8'))
                print("Sunucuya yollandı: " + data)  # Bilgiyi ekrana yazdır
            else:  # Eğer veri boş değilse
                print('Arduino Verisi: ' + arduinoData)  # Ekrana gelen veriyi yazdır
                s.send(arduinoData.encode('utf-8'))  # Sunucuya veriyi gönder
                print("Sunucuya yollandı: " + arduinoData)  # Bilgiyi ekrana yazdır

        s.close()  # Herhangi bir şekilde while döngüsünden çıkarsa sunucuya olan bağlantıyı kes
    except socket.error as e:  # Soket haberleşmede herhangi bir sorun çıkarsa
        print("[Server aktif değil.] Mesaj:", e)


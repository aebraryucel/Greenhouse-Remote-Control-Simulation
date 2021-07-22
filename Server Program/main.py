
import socket
import threading

ServerSocket = socket.socket()
host = "25.51.82.88"
port = 7777  # Port numarası

socketlist = {}


def get_key(socketobj): # verilen soketin hangi seraya ait olduğunu bulan fonksiyon
    for dictkey, sockets in socketlist.items():
        if socketobj == sockets:
            return dictkey

    return "Aranan eleman soket listesinde yok!"



#########################################

def threaded_sera(client):  # parametre olarak verilen client'dan alınan verinin yönetim programına aktaran fonksiyon

    yonetimSoketi = socketlist["yonetim"] # yönetime veri gönderebilmek için soket listesinde tuttuğumuz, bağlantının yapıldığı soketin listeden alınması

    clientId = get_key(client)

    if clientId == "sera1":  # eğer client sera1 ise sıcaklık değerinin başına s1 eklenip yönetime gönderilecek
        appendstring = "s1"

    elif clientId == "sera2":
        appendstring = "s2"

    elif clientId == "sera3":
        appendstring = "s3"

    sayac=0

    while True:
        try:
            sdata = client.recv(32)
        except:
            if sayac == 0:
                print(clientId + "in baglantisi kesildi")
                sayac=sayac+1
            continue

        if not sdata:
            continue
        else:
            try:
                decodedsdata=sdata.decode('utf-8')
                print("veri alındı : " + decodedsdata)
                send = appendstring + decodedsdata
                send = send.encode('utf-8')
                yonetimSoketi.send(send)
            except:
                yonetimSoketi = socketlist["yonetim"]
                continue


  #except:
    #print("Bağlantı kesildi")

#########################################


def threaded_yonetim(client):  # parametre olarak verilen yonetim soketinden komut beklenmesi,okunan komutun ilgili seraya aktarılması
  try:

    while True:
        data = client.recv(1024)
        if not data:
            continue
        else:
            decodedData = data.decode('utf-8')
            print(decodedData + " verisi alındı!")
            if decodedData == "cut1":  # eğer gelen veri cut1 ise 1. seranın kapatılacağı anlamına gelir.
                seraSocket = socketlist.get('sera1')  # ilgili seranın soketinin sözlükten alınması
                send = "999"  # sera kapatma kodunun oluşturulması
                encodedData = send.encode('utf-8')
                print("Sera Kapatma Kodu Gönderildi.")
                seraSocket.send(encodedData)  # sera kapatma kodunun ilgili seraya gönderilmesi

            elif decodedData == "cut2":
                seraSocket = socketlist.get('sera2')
                send = "999"
                encodedData = send.encode('utf-8')
                print("Sera Kapatma Kodu Gönderildi.")
                seraSocket.send(encodedData)

            elif decodedData == "cut3":
                seraSocket = socketlist.get('sera3')
                send = "999"
                encodedData = send.encode('utf-8')
                print("Sera Kapatma Kodu Gönderildi.")
                seraSocket.send(encodedData)

            elif decodedData[:7] == "degree1":
                seraSocket = socketlist.get('sera1')  # ilgili seranın soketinin sözlükten alınması
                degree = decodedData[7:]  # yönetimden gelecek sıcaklık değiştirme mesajı örnek olarak degree145 şeklinde olacaktır, degree1 sera1 içindir,sonraki karakterler sıcaklığı temsil eder
                encodedData = degree.encode('utf-8')  # degree1'den sonra gelen sıcaklık değeri kısmının ilgili seraya gönderilmesi
                seraSocket.send(encodedData)
                print("Sera Sıcaklığı Değiştirme İsteği Gönderildi.")

            elif decodedData[:7] == "degree2":
                seraSocket = socketlist.get('sera2')
                degree = decodedData[7:]
                encodedData = degree.encode('utf-8')
                seraSocket.send(encodedData)
                print("Sera Sıcaklığı Değiştirme İsteği Gönderildi.")

            elif decodedData[:7] == "degree3":
                seraSocket = socketlist.get('sera3')
                degree = decodedData[7:]
                encodedData = degree.encode('utf-8')
                seraSocket.send(encodedData)
                print("Sera Sıcaklığı Değiştirme İsteği Gönderildi.")

            else:
                print("Anlamsız veri girisi!")

  except:
    print("Bağlantı kesildi")





def listener():  #çalışma sırasında gelen bağlantıları bekleyen fonksiyon
    while True:
        Client, address = ServerSocket.accept()
        if Client and address:  # bağlantı geldiğinde
            identity = Client.recv(32)  # client'dan kimliğini yollamasını bekle
            decodedId = identity.decode('utf-8')
            print(decodedId + " bağlandı!")
            socketlist[decodedId]=Client
            if decodedId != "yonetim":
                t3 = threading.Thread(target=threaded_sera, args=[Client])
                t3.start()
            elif decodedId == "yonetim":
                t4 = threading.Thread(target=threaded_yonetim, args=[Client])
                t4.start()




if __name__ == '__main__':  ## main fonksiyon
    try:
        ServerSocket.bind((host, port))  #Sunucunun soketinin oluşturulması
    except socket.error as e:
        print(str(e))

    print('İstemciler bekleniyor...')
    ServerSocket.listen(4)

    while len(socketlist) != 4:  # Yönetim programı ve 3 adet sera bağlanana kadar döngü devam eder.Sera sayısına göre sayıyı arttırabiliriz.
        Client, address = ServerSocket.accept() #Gelen bağlantının kabul edilmesi
        if Client and address:  # bağlantı geldiğinde
            identity = Client.recv(32)  # client'dan kimliğini yollamasını bekle
            decodedId = identity.decode('utf-8')
            print(decodedId + " bağlandı!")
            socketlist[decodedId] = Client  # client'ı [Kimlik,Soket] şeklinde sözlüğe ekle. örnek:["yonetim":yonetimeAitSoket] veya ["sera1":sera1eAitSoket]

    for key, value in socketlist.items():
        print (key, value)

    t0 = threading.Thread(target=listener)  #gelen bağlantıların dinlenmesi için iş parçacığı başlatma
    t0.start()

    for key in socketlist.keys():
        if key != "yonetim":
            t1 = threading.Thread(target=threaded_sera, args=[socketlist.get(key)]) # Seralardan programından gelen verilerin okunması için iş parçacığı başlatılması
            t1.start()
        elif key == "yonetim":
            t2 = threading.Thread(target=threaded_yonetim, args=[socketlist.get(key)]) # Yönetim programından gelen verilerin okunması için iş parçacığı başlatılması
            t2.start()


#################################################




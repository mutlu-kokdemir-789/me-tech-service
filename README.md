
Backend:

Backend tarafinda servis calistirilidiginda https://localhost:7292 uzerinden hizmet vermektedir.

BookStore\appsettings.json içindeki 
- connection string i 'ConnectionStrings -> BookStore' kullanacağınız database in connection string i ile değiştiriniz.
- Eger 'BookStore' isminden farklı bir database tanımlamak isterseniz hem 'appsettings.json' içinden hem de Program.cs de 12. satirda değişiklik yapmaniz gerekmektedir.

- me-tech-service/BookStore/  dizinine gidin.
- Bu komutu çalıştırın:
    dotnet tool install --global dotnet-ef --version 8.*
- Ardından aşağıdaki komutu çalıştırın:
    dotnet add package Microsoft.EntityFrameworkCore.Design
- Daha sonra aşağıdaki komutu çalıştırın:
    dotnet build
- Daha sonra aşağıdaki komutu çalıştırın:
    dotnet ef database update
- Ardından aşağıdaki komutu çalıştırarak servisi ayağa kaldırabilirsiniz:
    dotnet run .\BookStore.csproj
- Eger 'Properties/launchSettings.json' icindeki 'profiles:http:applicationUrl' i değiştirmezseniz
    swagger UI a http://localhost:5162/swagger/index.html üzerinden erişebilirsiniz.

- Eğer Visual Studio ile çalıştırmak isterseniz projeyi Visual Studio da açın.
- Daha sonra Görünüm -> Diğer Pencereler -> Package Manager Console açın.
- Burada Add-Migration komutunu çalıştırın.
- Daha sonra Update-Database komutunu çalıştırın.
- Son olarak Çalıştır butonuna basarak ayaga kaldırabilirsiniz.





Frontend:

UI projesini ayaga kaldirmak icin gereken adimlar:
- me-tech-ui/ dizinine gidin.
- Aşağıdaki komutu çalıştırın:
    npm i
- Projeyi çalıştırmak için aşağıdaki komutu giriniz:
    npm run start
- Eger değişiklik yapmadıysanız varsayılan olarak uygulamaya http://localhost:4200/  üzerinden ulaşabilirsiniz. 
'npm run start' komutundan sonra terminalde de uygulamaya ulaşilacak url gözükmektedir.


- Uygulamanın işleyişi:

  - Üstteki naviagasyon barı:

    Üstteki navigasyon barında solda uygulama ismi olarak 'OBS APP' yazmaktadır. Bu yazıya tıklandığında da anasayfaya(kitapların listelendiği) gidilmektedir.
    Eger kullanıcı 'admin' ise üstteki bar da 'ADD BOOK' yazısı görmektedir.
    Bu yazıya tıkladığında 'Kitap Ekleme' sayfasına yönlendirilmektedir.
    Üstteki barın sağ tarafında ise kullanıcı ismi ve login/logut butonu yer almaktadır.
  
  - Kitapların listelendiği sayfa:

    Uygulamanin ana sayfasi kitaplarin listelendigi sayfadir.
    Bu sayfada filtreleme ve sıralama işlemi yapılabilmektedir. Ancak arama yapılamamaktadır.
    Kitap bilgilerinin altındaki çöp tenekesi ikonuna basılarak kitap silinebilmektedir.
    Ayrıca kitap imajlarına tıklandığında kitap detay sayfasına gidilmektedir.

  - Kitap ekleme sayfası:
    Bu sayfada kitap eklemek için bir form bulunmaktadır.
    Bu form aracılığıyla admin olan kullanıcı kitap ekleyebilmektedir

  - Kitap Detayı sayfası:
    Bu sayfada kitabın bilgileri ve 3 tane tab ı olan bir bar gözükmektedir.
    Comments tabında bu kitaba yapılan yorumlar listelenmektedir.
    Comment and Rate tabında kullanıcı kitaba yorum yapabilir veya puan verebilir. Bu tab sadece login olan kullanıcılara açıktır.
    Update tabında ise kullanıcı admin ise kitap bilgilerini değiştirebilir.

  - Login/Signup sayfası:
    Bu sayfada kullanıcı login olabilir veya kayıt olabilir.



Dipnot: Servis 'application url' i degistirilmek istenirse UI icinde 
  me-tech-ui\src\services\books.service.ts dosyasinda 18. satirda  ve me-tech-ui\src\services\authentication.service.ts
  dosyasinda 17. satirda url degisikligi yapilmasi gerekmektedir.



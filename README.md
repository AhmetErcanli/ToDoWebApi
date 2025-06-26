# ToDoWebApi

proje bir ToDo (Yapılacaklar Listesi) Web API’si. Temel olarak kullanıcıların kimlik doğrulaması yapabildiği ve yapılacaklar listesi üzerinde CRUD (Create, Read, Update, Delete) işlemleri gerçekleştirebildiği bir backend uygulaması. 

Aşağıda, bu programda neler yapılabileceğini ve bu işlemler sırasında hangi dosya ve fonksiyonların çalıştığını özetliyorum:

---

1. Kullanıcı Girişi (Authentication / Login)
- Amaç: Kullanıcı adı ve şifre ile giriş yapılır, başarılıysa JWT (token) ve refresh token döner.
- İlgili Dosyalar ve Fonksiyonlar:
  - `Controllers/AuthController.cs`: Giriş isteğini karşılar. (Muhtemelen bir `Login` veya `Authenticate` fonksiyonu var.)
  - `Services/AuthService.cs`: Kullanıcı doğrulama ve token üretimi işlemlerini yapar.
  - `Services/TokenService.cs`:
    - `CreateToken(User user)`: JWT token üretir.
    - `CreateRefreshToken()`: Refresh token üretir.
  - `Repositories/UserRepository.cs`: Kullanıcıyı veritabanında arar.

---

2. Token Doğrulama ve Yenileme
- Amaç: Kullanıcıdan gelen JWT token’ın geçerli olup olmadığını kontrol eder, süresi dolduysa refresh token ile yeniler.
- İlgili Dosyalar ve Fonksiyonlar:
  - `Middlewares/TokenRoleMiddleware.cs`: Gelen isteklerde token’ı kontrol eder, rol bazlı erişim sağlar.
  - `Services/TokenService.cs`:
    - `ValidateRefreshToken(User user, string refreshToken)`: Refresh token’ın geçerli olup olmadığını kontrol eder.

---

3. ToDo CRUD İşlemleri (Yapılacaklar Listesi)
- Amaç: Kullanıcılar yapılacaklar listesine yeni görev ekleyebilir, mevcut görevleri görebilir, güncelleyebilir veya silebilir.
- İlgili Dosyalar ve Fonksiyonlar:
- `Controllers/ToDoController.cs`: API üzerinden gelen ToDo ile ilgili istekleri karşılar (ör. `Get`, `Post`, `Put`, `Delete` fonksiyonları).
  - `Repositories/ToDoRepository.cs`: Veritabanı işlemlerini (ekle, sil, güncelle, listele) yapar.
  - `Models/ToDo.cs`: ToDo nesnesinin veri modelini tanımlar.

---

4. Hata Yönetimi
- Amaç: API’da oluşan hataları yakalar ve düzgün bir şekilde kullanıcıya iletir.
- İlgili Dosyalar ve Fonksiyonlar:
- `Middlewares/ExceptionMiddleware.cs`: Tüm isteklerde oluşan hataları yakalar ve uygun HTTP yanıtı döner.

---

5. Veritabanı İşlemleri
- Amaç: Kullanıcı ve ToDo verilerinin saklanması, güncellenmesi, silinmesi.
- İlgili Dosyalar ve Fonksiyonlar:
  - `AppDbContext.cs`: Entity Framework ile veritabanı bağlantısını ve tabloları yönetir.
  - `Migrations/`: Veritabanı şeması değişikliklerini içerir.

---

6. Konfigürasyon ve Ayarlar
- Amaç: Uygulamanın ayarlarını (ör. JWT anahtarı, veritabanı bağlantı dizesi) yönetir.
- İlgili Dosyalar:
  - `appsettings.json` ve `appsettings.Development.json`: Konfigürasyon dosyaları.

----------------------------------

Örnek Senaryo Akışı

1. Kullanıcı giriş yapar:  
   - `AuthController.Login` → `AuthService.Authenticate` → `UserRepository.GetByUsername` → `TokenService.CreateToken` & `TokenService.CreateRefreshToken`
2. Kullanıcı ToDo ekler: 
   - `ToDoController.Post` → `ToDoRepository.Add`
3. Kullanıcı ToDo’ları listeler:  
   - `ToDoController.Get` → `ToDoRepository.GetAll`
4. Token süresi dolarsa:  
   - Kullanıcı refresh token ile yeni JWT ister → `TokenService.ValidateRefreshToken` → `TokenService.CreateToken`

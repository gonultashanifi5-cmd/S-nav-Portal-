# Online Sınav Portalı

Ara sınav projesi, dersleri ve çoktan seçmeli sınavları yönetmek için hazırlanmış bir ASP.NET Core MVC uygulamasıdır. Yönetici giriş yaptıktan sonra ders, sınav ve soru ekleyebilir, düzenleyebilir ve silebilir. Soru işlemleri sınav detay ekranında AJAX ile çalışır.

## Visual Studio’da çalıştırma

1. Visual Studio 2026’da `OnlineSinavPortali.sln` dosyasını açın. Proje .NET 10 SDK ve **ASP.NET and web development** iş yükünü kullanır.
2. SQL Server’ın çalıştığından emin olun. Varsayılan bağlantı yerel SQL Server’a Windows kimliğiyle bağlanır. Başka bir sunucu veya bir LocalDB örneği kullanıyorsanız `ConnectionStrings:SqlServer` değerini **Manage User Secrets** üzerinden ayarlayabilirsiniz.
3. Yeni bir veritabanı kurarken **Tools → NuGet Package Manager → Package Manager Console** menüsünü açın. Varsayılan proje olarak `OnlineSinavPortali` seçip `Update-Database` komutunu çalıştırın.
4. Solution Explorer’da `OnlineSinavPortali` projesine sağ tıklayıp **Manage User Secrets** menüsünü açın. İlk yönetici henüz yoksa `AdminSetup` alanına kullanıcı adı ve 12–128 karakterli bir parola girin. Bu dosya kaynak kod deposunun dışında tutulur.

   ```json
   {
     "AdminSetup": {
       "UserName": "admin",
       "Password": "<kendi-parolanızı-yazın>"
     }
   }
   ```

5. **F5** ile `OnlineSinavPortali` HTTPS profilini başlatın. İlk çalıştırmada geliştirme ortamı kullanıcıyı oluşturur ve veritabanına parolanın özetini yazar. Mevcut yöneticinin parolası, bu başlangıç ayarı değiştirilerek güncellenmez.

## Proje yapısı

| Klasör | Görevi |
| --- | --- |
| `Models` | Veritabanı tabloları ve tabloların ilişkileri |
| `ViewModels` | Form alanları, doğrulama kuralları ve ekran verileri |
| `Repositories` | Ortak veri işlemleri ve sınava ait soru sorguları |
| `Controllers` | Giriş, yetkilendirme, yönetim ekranları ve AJAX uçları |
| `Views` | Razor sayfaları, yönetici paneli ve soru formu |
| `Migrations` | SQL Server şemasının sürümlü değişiklikleri |
| `wwwroot/js/questions.js` | Soru formunu gönderme ve listeyi yenileme |

`uy01` ders örneğindeki MVC, `AppDbContext`, `GenericRepository<T>` ve yönetici yerleşimi yapısı bu projeye uyarlandı. Razor sayfaları arşivdeki Bootstrap 5.1.0 ve ortak site stillerini kullanır. Veritabanı şeması SQL Server için Entity Framework Core Code First migration’larıyla oluşturulur.

## Piyasa örnekleri

- Microsoft Forms, çoktan seçmeli sorularda doğru cevap ve puan belirlemeye izin verir. Portalın yönetici ekranında her sorunun dört seçeneği, bir doğru cevabı ve bir puan değeri bulunur. [Microsoft Forms sınav oluşturma kılavuzu](https://support.microsoft.com/en-us/forms/create-a-quiz-with-microsoft-forms)
- Moodle, süre sınırı ve öğrenci için geri sayım sunar. Portalda yönetici sınav süresini dakika olarak kaydeder. Finalde öğrenci ekranı bu süreyi geri sayım için kullanacak ve sınav tesliminde sunucu süreyi denetleyecek. [Moodle sınav ayarları](https://docs.moodle.org/501/en/Quiz_settings)

## Teslim kapsamı

- **Ara sınav** için veritabanı tasarımı, model ve ViewModel’ler, SQL Server bağlantısı, migration, repository deseni, yönetici paneli, cookie ile giriş ve yetkilendirme, yönetim sayfaları ve AJAX soru işlemleri hazır. GitHub’a kaynak kod yüklemesi teslim adımı olarak bekliyor.
- **Final** için genel öğrenci arayüzü, sınava katılma ve sonuç görüntüleme, ASP.NET Core Identity ile üyelik ve rol yönetimi, yönetici panelinde SignalR ve GitHub güncellemesi planlandı.
- **Proje sonunda** genel yapıyı, temel kodları ve uygulamanın işleyişini gösteren en fazla 5 dakikalık bir tanıtım videosu hazırlanacak.

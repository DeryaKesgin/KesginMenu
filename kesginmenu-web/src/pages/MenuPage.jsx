import { useEffect, useState } from "react";
import { useParams } from "react-router";
import api from "../api/api";
import "./MenuPage.css";

function MenuPage() {
  const { slug } = useParams();

  const [menu, setMenu] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadMenu() {
      setLoading(true);
      setError("");

      try {
        const response = await api.get(`/Menu/${slug}`);
        setMenu(response.data);
      } catch (requestError) {
        console.error("Menü yükleme hatası:", requestError);

        setMenu(null);

        setError(
          requestError.response?.data?.message ??
            "Menü yüklenirken bir sorun oluştu."
        );
      } finally {
        setLoading(false);
      }
    }

    if (slug) {
      loadMenu();
    } else {
      setLoading(false);
      setError("Geçerli bir menü bağlantısı bulunamadı.");
    }
  }, [slug]);

  if (loading) {
    return (
      <div className="status-message">
        Menü yükleniyor...
      </div>
    );
  }

  if (error) {
    return (
      <div className="status-message">
        {error}
      </div>
    );
  }

  if (!menu) {
    return (
      <div className="status-message">
        Menü bulunamadı.
      </div>
    );
  }

  return (
    <main className="menu-page">
      <header className="menu-header">
        {menu.logoUrl && (
          <img
            className="business-logo"
            src={menu.logoUrl}
            alt={`${menu.businessName} logosu`}
          />
        )}

        <h1>{menu.businessName}</h1>

        {menu.description && (
          <p>{menu.description}</p>
        )}
      </header>

      <nav className="category-navigation">
        {menu.categories.map((category) => (
          <a
            key={category.id}
            href={`#category-${category.id}`}
          >
            {category.name}
          </a>
        ))}
      </nav>

      <section className="menu-content">
        {menu.categories.length === 0 && (
          <p className="empty-category">
            Bu menüde henüz kategori bulunmuyor.
          </p>
        )}

        {menu.categories.map((category) => (
          <section
            className="category-section"
            id={`category-${category.id}`}
            key={category.id}
          >
            <h2>{category.name}</h2>

            <div className="product-list">
              {category.products.length === 0 && (
                <p className="empty-category">
                  Bu kategoride henüz ürün bulunmuyor.
                </p>
              )}

              {category.products.map((product) => (
                <article
                  className="product-card"
                  key={product.id}
                >
                  {product.imageUrl && (
                    <img
                      src={product.imageUrl}
                      alt={product.name}
                      className="product-image"
                    />
                  )}

                  <div className="product-details">
                    <div className="product-heading">
                      <h3>{product.name}</h3>

                      <strong>
                        {Number(product.price).toLocaleString(
                          "tr-TR",
                          {
                            style: "currency",
                            currency: "TRY",
                          }
                        )}
                      </strong>
                    </div>

                    {product.description && (
                      <p>{product.description}</p>
                    )}
                  </div>
                </article>
              ))}
            </div>
          </section>
        ))}
      </section>
    </main>
  );
}

export default MenuPage;
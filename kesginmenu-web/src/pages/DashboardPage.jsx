import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router";
import api from "../api/api";
import "./DashboardPage.css";

function DashboardPage() {
  const navigate = useNavigate();

  const user = JSON.parse(
    localStorage.getItem("user") ?? "{}"
  );

  const [dashboardData, setDashboardData] = useState({
    categoryCount: 0,
    productCount: 0,
    businessName: "",
    slug: "",
    isActive: false,
  });

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadDashboard = async () => {
      if (!user.businessId) {
        setError("İşletme bilgisi bulunamadı.");
        setLoading(false);
        return;
      }

      try {
        const [
          businessResponse,
          categoriesResponse,
          productsResponse,
        ] = await Promise.all([
          api.get(`/Businesses/${user.businessId}`),
          api.get(
            `/Categories/business/${user.businessId}`
          ),
          api.get(
            `/Products/business/${user.businessId}`
          ),
        ]);

        setDashboardData({
          categoryCount:
            categoriesResponse.data.length,
          productCount:
            productsResponse.data.length,
          businessName:
            businessResponse.data.name,
          slug:
            businessResponse.data.slug,
          isActive:
            businessResponse.data.isActive,
        });
      } catch (requestError) {
        console.error(requestError);
        setError("Panel bilgileri yüklenemedi.");
      } finally {
        setLoading(false);
      }
    };

    loadDashboard();
  }, [user.businessId]);

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");

    navigate("/login", {
      replace: true,
    });
  };

  const menuPath = dashboardData.slug
    ? `/menu/${dashboardData.slug}`
    : "#";

  return (
    <main className="dashboard-page">
      <aside className="dashboard-sidebar">
        <div>
          <span className="dashboard-brand">
            KesginSoft
          </span>

          <h2>QR Menü</h2>
        </div>

        <nav>
          <Link
            className="active"
            to="/panel"
          >
            Genel Bakış
          </Link>

          <Link to="/panel/kategoriler">
            Kategoriler
          </Link>

          <Link to="/panel/urunler">
            Ürünler
          </Link>

          <Link to="/panel/qr">
            QR Kod
          </Link>

          {dashboardData.slug && (
            <Link
              to={menuPath}
              target="_blank"
              rel="noreferrer"
            >
              Menüyü Görüntüle
            </Link>
          )}
        </nav>

        <button
          type="button"
          className="logout-button"
          onClick={handleLogout}
        >
          Çıkış Yap
        </button>
      </aside>

      <section className="dashboard-content">
        <header className="dashboard-header">
          <div>
            <p>Hoş geldiniz</p>

            <h1>
              {user.fullName ?? "Kullanıcı"}
            </h1>
          </div>

          {dashboardData.slug && (
            <Link
              to={menuPath}
              target="_blank"
              rel="noreferrer"
            >
              Menüyü Aç
            </Link>
          )}
        </header>

        <section className="dashboard-introduction">
          <h2>
            {dashboardData.businessName ||
              "Menü Yönetimi"}
          </h2>

          <p>
            Kategorilerinizi, ürünlerinizi ve QR
            kodunuzu buradan yönetebilirsiniz.
          </p>
        </section>

        {error && (
          <p className="management-message">
            {error}
          </p>
        )}

        <section className="dashboard-cards">
          <article>
            <span>Kategoriler</span>

            <strong>
              {loading
                ? "..."
                : dashboardData.categoryCount}
            </strong>

            <p>
              Menü bölümünüz bulunuyor.
            </p>
          </article>

          <article>
            <span>Ürünler</span>

            <strong>
              {loading
                ? "..."
                : dashboardData.productCount}
            </strong>

            <p>
              Menünüzde kayıtlı ürün bulunuyor.
            </p>
          </article>

          <article>
            <span>Menü Durumu</span>

            <strong>
              {loading
                ? "..."
                : dashboardData.isActive
                  ? "Aktif"
                  : "Kapalı"}
            </strong>

            <p>
              {dashboardData.isActive
                ? "QR menünüz ziyaretçilere açık."
                : "QR menünüz şu anda kapalı."}
            </p>
          </article>
        </section>
      </section>
    </main>
  );
}

export default DashboardPage;
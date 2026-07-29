import { useState } from "react";
import { useNavigate } from "react-router";
import api from "../api/api";
import "./LoginPage.css";

function LoginPage() {
  const navigate = useNavigate();

  const [form, setForm] = useState({
    email: "",
    password: "",
  });

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleChange = (event) => {
    const { name, value } = event.target;

    setForm((current) => ({
      ...current,
      [name]: value,
    }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    setLoading(true);
    setError("");

    try {
      const response = await api.post("/Auth/login", form);

      const loginData = response.data;

      localStorage.setItem("token", loginData.token);

      localStorage.setItem(
        "user",
        JSON.stringify({
          userId: loginData.userId,
          businessId: loginData.businessId,
          fullName: loginData.fullName,
          email: loginData.email,
          role: loginData.role,
          expiresAt: loginData.expiresAt,
        })
      );

      navigate("/panel", { replace: true });
    } catch (requestError) {
      setError(
        requestError.response?.data?.message ??
          "Giriş yapılırken bir sorun oluştu."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="login-page">
      <section className="login-card">
        <div className="login-brand">
          <span>KesginSoft</span>
          <h1>QR Menü Yönetim Paneli</h1>
          <p>
            Menü içeriklerinizi kolayca yönetin ve değişiklikleri anında
            yayınlayın.
          </p>
        </div>

        <form className="login-form" onSubmit={handleSubmit}>
          <label>
            E-posta
            <input
              type="email"
              name="email"
              value={form.email}
              onChange={handleChange}
              autoComplete="email"
              required
            />
          </label>

          <label>
            Şifre
            <input
              type="password"
              name="password"
              value={form.password}
              onChange={handleChange}
              autoComplete="current-password"
              required
            />
          </label>

          {error && <div className="login-error">{error}</div>}

          <button type="submit" disabled={loading}>
            {loading ? "Giriş yapılıyor..." : "Giriş Yap"}
          </button>
        </form>

        
      </section>
    </main>
  );
}

export default LoginPage;
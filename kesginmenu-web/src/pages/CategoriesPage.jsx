import { useEffect, useState } from "react";
import { Link } from "react-router";
import api from "../api/api";
import "./ManagementPage.css";

function CategoriesPage() {
  const user = JSON.parse(localStorage.getItem("user") ?? "{}");

  const [categories, setCategories] = useState([]);
  const [name, setName] = useState("");
  const [editingCategory, setEditingCategory] = useState(null);
  const [editName, setEditName] = useState("");
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(true);

  const loadCategories = async () => {
    try {
      const response = await api.get(
        `/Categories/business/${user.businessId}`
      );

      setCategories(response.data);
    } catch {
      setMessage("Kategoriler yüklenemedi.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCategories();
  }, []);

  const handleSubmit = async (event) => {
    event.preventDefault();

    if (!name.trim()) {
      return;
    }

    try {
      await api.post("/Categories", {
        name,
        displayOrder: categories.length + 1,
        businessId: user.businessId,
      });

      setName("");
      setMessage("Kategori başarıyla eklendi.");
      await loadCategories();
    } catch (error) {
      setMessage(
        error.response?.data?.message ??
          "Kategori eklenirken bir sorun oluştu."
      );
    }
  };

  const startEditing = (category) => {
    setEditingCategory(category);
    setEditName(category.name);
    setMessage("");
  };

  const cancelEditing = () => {
    setEditingCategory(null);
    setEditName("");
  };

  const handleUpdate = async () => {
    if (!editName.trim() || !editingCategory) {
      return;
    }

    try {
      await api.put(`/Categories/${editingCategory.id}`, {
        name: editName,
        displayOrder: editingCategory.displayOrder,
        isActive: editingCategory.isActive,
      });

      setMessage("Kategori başarıyla güncellendi.");
      cancelEditing();
      await loadCategories();
    } catch (error) {
      setMessage(
        error.response?.data?.message ??
          "Kategori güncellenirken bir sorun oluştu."
      );
    }
  };

  const handleDelete = async (id) => {
    const confirmed = window.confirm(
      "Bu kategori ve içindeki ürünler silinecek. Emin misiniz?"
    );

    if (!confirmed) {
      return;
    }

    try {
      await api.delete(`/Categories/${id}`);
      setMessage("Kategori silindi.");
      await loadCategories();
    } catch (error) {
      setMessage(
        error.response?.data?.message ??
          "Kategori silinirken bir sorun oluştu."
      );
    }
  };

  return (
    <main className="management-page">
      <header className="management-header">
        <div>
          <Link to="/panel">← Panele dön</Link>
          <h1>Kategoriler</h1>
          <p>Menünüzde yer alacak bölümleri yönetin.</p>
        </div>
      </header>

      <section className="management-card">
        <form className="management-form" onSubmit={handleSubmit}>
          <input
            type="text"
            placeholder="Kategori adı"
            value={name}
            onChange={(event) => setName(event.target.value)}
          />

          <button type="submit">Kategori Ekle</button>
        </form>

        {message && <p className="management-message">{message}</p>}

        {loading ? (
          <p>Yükleniyor...</p>
        ) : (
          <div className="management-list">
            {categories.map((category) => (
              <article key={category.id}>
                {editingCategory?.id === category.id ? (
                  <div className="inline-edit">
                    <input
                      value={editName}
                      onChange={(event) =>
                        setEditName(event.target.value)
                      }
                    />

                    <button
                      type="button"
                      className="save-button"
                      onClick={handleUpdate}
                    >
                      Kaydet
                    </button>

                    <button
                      type="button"
                      className="secondary-button"
                      onClick={cancelEditing}
                    >
                      Vazgeç
                    </button>
                  </div>
                ) : (
                  <>
                    <div>
                      <strong>{category.name}</strong>
                      <span>Sıra: {category.displayOrder}</span>
                    </div>

                    <div className="action-buttons">
                      <button
                        type="button"
                        className="edit-button"
                        onClick={() => startEditing(category)}
                      >
                        Düzenle
                      </button>

                      <button
                        type="button"
                        className="delete-button"
                        onClick={() => handleDelete(category.id)}
                      >
                        Sil
                      </button>
                    </div>
                  </>
                )}
              </article>
            ))}
          </div>
        )}
      </section>
    </main>
  );
}

export default CategoriesPage;
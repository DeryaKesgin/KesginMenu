import { useEffect, useState } from "react";
import { Link } from "react-router";
import api from "../api/api";
import "./ManagementPage.css";

const emptyForm = {
  name: "",
  description: "",
  price: "",
  imageUrl: "",
  categoryId: "",
};

function ProductsPage() {
  const user = JSON.parse(localStorage.getItem("user") ?? "{}");

  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [form, setForm] = useState(emptyForm);
  const [editingProduct, setEditingProduct] = useState(null);
  const [message, setMessage] = useState("");
  const [imageUploading, setImageUploading] = useState(false);
  const [saving, setSaving] = useState(false);

  const loadData = async () => {
    try {
      const [productsResponse, categoriesResponse] =
        await Promise.all([
          api.get(`/Products/business/${user.businessId}`),
          api.get(`/Categories/business/${user.businessId}`),
        ]);

      setProducts(productsResponse.data);
      setCategories(categoriesResponse.data);
    } catch (error) {
      console.error(error);
      setMessage("Ürünler yüklenemedi.");
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleChange = (event) => {
    const { name, value } = event.target;

    setForm((current) => ({
      ...current,
      [name]: value,
    }));
  };

  const handleImageUpload = async (event) => {
    const file = event.target.files?.[0];

    if (!file) {
      return;
    }

    const allowedTypes = [
      "image/jpeg",
      "image/png",
      "image/webp",
    ];

    if (!allowedTypes.includes(file.type)) {
      setMessage(
        "Yalnızca JPG, PNG veya WEBP görsel seçebilirsiniz."
      );

      event.target.value = "";
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      setMessage("Görsel boyutu en fazla 5 MB olabilir.");
      event.target.value = "";
      return;
    }

    const formData = new FormData();
    formData.append("file", file);

    setImageUploading(true);
    setMessage("");

    try {
      const response = await api.post("/Upload", formData);

      console.log("Yüklenen görsel:", response.data);

      const uploadedImageUrl = response.data?.imageUrl;

      if (!uploadedImageUrl) {
        throw new Error(
          "Sunucudan görsel bağlantısı alınamadı."
        );
      }

      setForm((current) => ({
        ...current,
        imageUrl: uploadedImageUrl,
      }));

      setMessage(
        "Görsel başarıyla yüklendi. Şimdi ürünü kaydedebilirsiniz."
      );
    } catch (error) {
      console.error("Görsel yükleme hatası:", error);

      const errorMessage =
        typeof error.response?.data === "string"
          ? error.response.data
          : error.response?.data?.message;

      setMessage(
        errorMessage ??
          error.message ??
          "Görsel yüklenirken bir sorun oluştu."
      );
    } finally {
      setImageUploading(false);
      event.target.value = "";
    }
  };

  const resetForm = () => {
    setForm({ ...emptyForm });
    setEditingProduct(null);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    if (imageUploading) {
      setMessage(
        "Lütfen görsel yüklemesinin tamamlanmasını bekleyin."
      );
      return;
    }

    if (!form.name.trim()) {
      setMessage("Ürün adı zorunludur.");
      return;
    }

    if (!form.categoryId) {
      setMessage("Lütfen bir kategori seçin.");
      return;
    }

    const payload = {
      name: form.name.trim(),
      description: form.description.trim() || null,
      price: Number(form.price),
      imageUrl: form.imageUrl || null,
      displayOrder:
        editingProduct?.displayOrder ?? products.length + 1,
      categoryId: Number(form.categoryId),
    };

    setSaving(true);
    setMessage("");

    try {
      if (editingProduct) {
        await api.put(`/Products/${editingProduct.id}`, {
          ...payload,
          isAvailable: editingProduct.isAvailable,
        });

        setMessage("Ürün başarıyla güncellendi.");
      } else {
        await api.post("/Products", payload);
        setMessage("Ürün başarıyla eklendi.");
      }

      resetForm();
      await loadData();
    } catch (error) {
      console.error("Ürün kaydetme hatası:", error);

      setMessage(
        error.response?.data?.message ??
          "İşlem sırasında bir sorun oluştu."
      );
    } finally {
      setSaving(false);
    }
  };

  const startEditing = (product) => {
    setEditingProduct(product);

    setForm({
      name: product.name ?? "",
      description: product.description ?? "",
      price: product.price ?? "",
      imageUrl: product.imageUrl ?? "",
      categoryId: String(product.categoryId),
    });

    setMessage("");

    window.scrollTo({
      top: 0,
      behavior: "smooth",
    });
  };

  const handleDelete = async (id) => {
    const confirmed = window.confirm(
      "Bu ürünü silmek istediğinize emin misiniz?"
    );

    if (!confirmed) {
      return;
    }

    try {
      await api.delete(`/Products/${id}`);

      if (editingProduct?.id === id) {
        resetForm();
      }

      setMessage("Ürün silindi.");
      await loadData();
    } catch (error) {
      console.error(error);

      setMessage(
        error.response?.data?.message ??
          "Ürün silinirken bir sorun oluştu."
      );
    }
  };

  return (
    <main className="management-page">
      <header className="management-header">
        <div>
          <Link to="/panel">← Panele dön</Link>

          <h1>Ürünler</h1>

          <p>Menünüzdeki ürünleri yönetin.</p>
        </div>
      </header>

      <section className="management-card">
        <form
          className="management-form product-form"
          onSubmit={handleSubmit}
        >
          <input
            name="name"
            placeholder="Ürün adı"
            value={form.name}
            onChange={handleChange}
            required
          />

          <input
            name="description"
            placeholder="Ürün açıklaması"
            value={form.description}
            onChange={handleChange}
          />

          <input
            type="number"
            name="price"
            placeholder="Fiyat"
            value={form.price}
            onChange={handleChange}
            min="0"
            step="0.01"
            required
          />

          <label
            className={`image-upload-button ${
              imageUploading ? "uploading" : ""
            }`}
          >
            <span>
              {imageUploading
                ? "Görsel yükleniyor..."
                : form.imageUrl
                  ? "Görseli Değiştir"
                  : "Görsel Seç"}
            </span>

            <input
              type="file"
              accept="image/jpeg,image/png,image/webp"
              onChange={handleImageUpload}
              disabled={imageUploading || saving}
            />
          </label>

          <select
            name="categoryId"
            value={form.categoryId}
            onChange={handleChange}
            required
          >
            <option value="">Kategori seçin</option>

            {categories.map((category) => (
              <option
                key={category.id}
                value={category.id}
              >
                {category.name}
              </option>
            ))}
          </select>

          <button
            type="submit"
            disabled={imageUploading || saving}
          >
            {imageUploading
              ? "Görsel yükleniyor..."
              : saving
                ? "Kaydediliyor..."
                : editingProduct
                  ? "Değişiklikleri Kaydet"
                  : "Ürün Ekle"}
          </button>

          {editingProduct && (
            <button
              type="button"
              className="secondary-button"
              onClick={resetForm}
              disabled={imageUploading || saving}
            >
              Vazgeç
            </button>
          )}
        </form>

        {form.imageUrl && (
          <div className="image-preview">
            <img
              src={form.imageUrl}
              alt="Seçilen ürün görseli"
              onError={(event) => {
                event.currentTarget.style.display = "none";
                setMessage(
                  "Görsel bağlantısı açılmadı. Yeniden yüklemeyi deneyin."
                );
              }}
            />

            <div>
              <strong>Görsel hazır</strong>

              <span>
                Ürünü kaydettiğinizde menüde
                yayınlanacaktır.
              </span>
            </div>

            <button
              type="button"
              className="delete-button"
              onClick={() =>
                setForm((current) => ({
                  ...current,
                  imageUrl: "",
                }))
              }
              disabled={saving}
            >
              Görseli Kaldır
            </button>
          </div>
        )}

        {message && (
          <p className="management-message">
            {message}
          </p>
        )}

        <div className="management-list product-management-list">
          {products.map((product) => (
            <article key={product.id}>
              <div className="product-management-info">
                {product.imageUrl && (
                  <img
                    src={product.imageUrl}
                    alt={product.name}
                    className="management-product-image"
                  />
                )}

                <div>
                  <strong>{product.name}</strong>

                  <span>
                    {product.categoryName} ·{" "}
                    {Number(product.price).toLocaleString(
                      "tr-TR",
                      {
                        style: "currency",
                        currency: "TRY",
                      }
                    )}
                  </span>

                  {product.description && (
                    <small>{product.description}</small>
                  )}
                </div>
              </div>

              <div className="action-buttons">
                <button
                  type="button"
                  className="edit-button"
                  onClick={() => startEditing(product)}
                >
                  Düzenle
                </button>

                <button
                  type="button"
                  className="delete-button"
                  onClick={() => handleDelete(product.id)}
                >
                  Sil
                </button>
              </div>
            </article>
          ))}
        </div>
      </section>
    </main>
  );
}

export default ProductsPage;
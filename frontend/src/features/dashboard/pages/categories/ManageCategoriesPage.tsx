import React, { useState, useEffect } from 'react';
import categoryService from '../../../../services/categoryService';
import './ManageCategoriesPage.css'; 

interface Category {
  categoryID: number;
  name: string;
}

const ManageCategoriesPage = () => {
  const [categories, setCategories] = useState<Category[]>([]);
  const [newCategoryName, setNewCategoryName] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    fetchCategories();
  }, []);

  const fetchCategories = () => {
    categoryService.getCategories()
      .then(response => {
        setCategories(response.data);
      })
      .catch(err => {
        setError('Greška pri učitavanju kategorija.');
      });
  };

  const handleCreateCategory = async (e: React.FormEvent) => {
    e.preventDefault();
    if (newCategoryName.trim() === '') return;
    setError(''); 

    try {
      await categoryService.createCategory(newCategoryName);
      setNewCategoryName(''); 
      fetchCategories(); 
    } catch (error: any) {
      if (error.response && error.response.data) {
        setError(error.response.data);
      } else {
        setError('Došlo je do greške. Molimo pokušajte ponovo.');
      }
    }
  };

  const handleDeleteCategory = async (id: number) => {
    if (window.confirm('Da li ste sigurni da želite da obrišete ovu kategoriju?')) {
      try {
        await categoryService.deleteCategory(id);
        fetchCategories(); 
      } catch (err) {
        setError('Greška pri brisanju kategorije.');
      }
    }
  };

  return (
    <div className="manage-categories-page">
      <h2>Upravljanje Kategorijama</h2>

      <form onSubmit={handleCreateCategory} className="add-category-form">
        <input 
          type="text"
          value={newCategoryName}
          onChange={(e) => setNewCategoryName(e.target.value)}
          placeholder="Unesite ime nove kategorije"
        />
        <button type="submit" className="btn">Dodaj</button>
      </form>

      {error && <p className="error-message">{error}</p>}

      <ul className="categories-list">
        {categories.map(category => (
          <li key={category.categoryID}>
            <span>{category.name}</span>
            <div className="category-actions">
              <button onClick={() => handleDeleteCategory(category.categoryID)} className="btn-delete">Obriši</button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ManageCategoriesPage;
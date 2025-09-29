import React, { useState, useEffect } from 'react';
import categoryService from '../../../../../../services/categoryService';
import Select from 'react-select'; 
import './QuizForm.css'; 

interface QuizFormProps {
  quizData: any;
  onDataChange: (field: string, value: any) => void;
  mode?: 'solo' | 'live';
}

interface Category {
  categoryID: number;
  name: string;
}

const QuizForm: React.FC<QuizFormProps> = ({ quizData, onDataChange, mode }) => {
  const [allCategories, setAllCategories] = useState([]);

  useEffect(() => {
    categoryService.getCategories().then((response) => {
      const options = response.data.map((cat: Category) => ({
        value: cat.categoryID,
        label: cat.name
      }));
      setAllCategories(options);
    });
  }, []);

  const handleSelectChange = (selectedOptions: any) => {
    onDataChange('categoryIds', selectedOptions || []);
  };

  return (
     <div className="quiz-form-card">
      <h3>Detalji Kviza</h3>

      <div className="quiz-form-group">
        <label>Naziv Kviza</label>
        <input 
          type="text" 
          value={quizData.name || ''} 
          onChange={e => onDataChange('name', e.target.value)} 
          required
        />
      </div>

      <div className="quiz-form-group">
        <label>Opis</label>
        <textarea 
          value={quizData.description || ''} 
          onChange={e => onDataChange('description', e.target.value)} 
        />
      </div>

      {mode === 'solo' && (
        <div className="quiz-form-group">
          <label>Vreme (u sekundama)</label>
          <input 
            type="number" 
            value={quizData.timeLimit || 300} 
            onChange={e => onDataChange('timeLimit', parseInt(e.target.value))} 
            required
          />
        </div>
      )}
      
      <div className="quiz-form-group">
        <label>Težina</label>
        <select 
          value={quizData.difficulty || 0} 
          onChange={e => onDataChange('difficulty', parseInt(e.target.value))}
        >
          <option value={0}>Lako</option>
          <option value={1}>Srednje</option>
          <option value={2}>Teško</option>
        </select>
      </div>

      <div className="quiz-form__group">
        <label>Kategorije</label>
        <Select
          options={allCategories}
          isMulti 
          className="react-select-container"
          classNamePrefix="react-select"
          placeholder="Pretraži i izaberi kategorije..."
          value={quizData.categoryIds}
          onChange={handleSelectChange}
          required
        />
     </div>
    </div>
  );
};

export default QuizForm;
import React, { useState, useEffect } from 'react';
import './QuestionForm.css';

const ThreeDotsIcon: React.FC<React.SVGProps<SVGSVGElement>> = (props) => {
  return (
    <svg 
      xmlns="http://www.w3.org/2000/svg" 
      fill="none" 
      viewBox="0 0 24 24" 
      strokeWidth={1.5} 
      stroke="currentColor" 
      {...props} 
    >
      <path 
        strokeLinecap="round" 
        strokeLinejoin="round" 
        d="M12 6.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 12.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 18.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5Z" 
      />
    </svg>
  );
};

interface QuestionFormProps {
  questionData: any;
  onDataChange: (questionIndex: number, updatedQuestionData: any) => void; 
  onRemove: (questionIndex: number) => void;
  questionIndex: number;
  onAddOption: () => void;
}

const QuestionForm: React.FC<QuestionFormProps> = ({ questionData, questionIndex, onDataChange, onRemove, onAddOption }) => {
  const [displayValue, setDisplayValue] = useState(String(questionData.pointNum || '').replace('.', ','));
  const [isTypeMenuOpen, setTypeMenuOpen] = useState(false);

  useEffect(() => {
    setDisplayValue(String(questionData.pointNum || '').replace('.', ','));
  }, [questionData.pointNum]);

  const handleChange = (field: string, value: any) => {
    onDataChange(questionIndex, { ...questionData, [field]: value });
  };

  const handleAnswerChange = (answerIndex: number, field: string, value: any) => {
    const newAnswerOptions = [...questionData.answerOptions];
    newAnswerOptions[answerIndex][field] = value;
    handleChange('answerOptions', newAnswerOptions);
  };

  const handleCorrectAnswerChange = (answerIndex: number) => {
    let newAnswerOptions;
    if (questionData.type === 0 || questionData.type === 2) { 
        newAnswerOptions = questionData.answerOptions.map((opt: any, i: number) => ({
            ...opt, isCorrect: i === answerIndex
        }));
    } else { 
        newAnswerOptions = questionData.answerOptions.map((opt: any, i: number) => ({
            ...opt, isCorrect: i === answerIndex ? !opt.isCorrect : opt.isCorrect
        }));
    }
    handleChange('answerOptions', newAnswerOptions);
  };

  const removeAnswerOption = (answerIndex: number) => {
    if (questionData.answerOptions.length <= 2) return;
    const newAnswerOptions = questionData.answerOptions.filter((_: any, i: number) => i !== answerIndex);
    handleChange('answerOptions', newAnswerOptions);
  };

  const handleTypeSelect = (newType: number) => {
    const oldType = questionData.type;

    if (newType === oldType) {
      setTypeMenuOpen(false);
      return;
    }
        
    const newQuestionData = { ...questionData, type: newType };

    const isSwitchingToFillIn = newType === 3;
    const isSwitchingFromFillIn = oldType === 3;

    if (isSwitchingToFillIn) {
      newQuestionData.answerOptions = [];
    } else if (isSwitchingFromFillIn) {
      newQuestionData.correctTextAnswer = '';
      newQuestionData.answerOptions = newType === 2
        ? [{ text: 'Tačno', isCorrect: true }, { text: 'Netačno', isCorrect: false }]
        : [{ text: '', isCorrect: false }, { text: '', isCorrect: false }];
    } else {
      if (newType === 2) {
          newQuestionData.answerOptions = [{ text: 'Tačno', isCorrect: true }, { text: 'Netačno', isCorrect: false }];
      } else if (oldType === 2) {
          newQuestionData.answerOptions = [{ text: '', isCorrect: false }, { text: '', isCorrect: false }];
      } else {
        let options = [...questionData.answerOptions];
        if (oldType === 1 && newType === 0) {
          let foundFirst = false;
          options = options.map(opt => {
            if (opt.isCorrect && !foundFirst) {
              foundFirst = true;
              return opt; 
            }
            return { ...opt, isCorrect: false }; 
          });
        }
        newQuestionData.answerOptions = options;
      }
    }

    onDataChange(questionIndex, newQuestionData);
    setTypeMenuOpen(false);
  };

  return (
    <div className="question-form-card">
      <div className="question-form-header">
        <h3>Pitanje #{questionIndex + 1}</h3>
        <div className="question-form-header-actions">
          <div className="type-menu-container">
            <button type="button" onClick={() => setTypeMenuOpen(!isTypeMenuOpen)} className="type-menu-trigger">
              <ThreeDotsIcon className="dots-icon"/>
            </button>
            {isTypeMenuOpen && (
              <div className="type-menu-dropdown">
                <button type="button" onClick={() => handleTypeSelect(0)}>Jedan tačan</button>
                <button type="button" onClick={() => handleTypeSelect(1)}>Više tačnih</button>
                <button type="button" onClick={() => handleTypeSelect(2)}>Tačno/Netačno</button>
                <button type="button" onClick={() => handleTypeSelect(3)}>Unos teksta</button>
              </div>
            )}
          </div>
          <button type="button" onClick={() => onRemove(questionIndex)} className="remove-question-btn">
            Obrisi
          </button>
        </div>
      </div>

      <div className="question-form-group">
        <label>Tekst pitanja</label>
        <textarea value={questionData.questionText || ''} onChange={e => handleChange('questionText', e.target.value)} required />
      </div>

      {questionData.type === 3 && (
         <div className="question-form-group">
            <label>Tačan tekstualni odgovor</label>
            <input type="text" value={questionData.correctTextAnswer || ''} onChange={e => handleChange('correctTextAnswer', e.target.value)} required />
          </div>
      )}

      {questionData.type < 3 && (
        <div className="question-form-group">
          <label>Ponuđeni odgovori</label>
            {questionData.answerOptions.map((opt: any, index: number) => (
              <div key={index} className="question-form-answer-option">
                <label className="custom-control-label">
                  <input 
                    type={questionData.type === 0 || questionData.type === 2 ? "radio" : "checkbox"} 
                    name={`correctAnswer-${questionIndex}`} 
                    checked={opt.isCorrect}
                    onChange={() => handleCorrectAnswerChange(index)}
                  />
                  <span className="custom-control-indicator"></span>
                </label>
                <input 
                  type="text" 
                  placeholder={`Odgovor ${index + 1}`}
                  value={opt.text}
                  onChange={e => handleAnswerChange(index, 'text', e.target.value)}
                  required
                  disabled={questionData.type === 2} 
                />
                {questionData.type < 2 && ( 
                  <button type="button" onClick={() => removeAnswerOption(index)} className="question-remove-option-btn">
                    Obrisi
                  </button>
                )}
              </div>
            ))}
            {questionData.type < 2 && ( 
              <button type="button" onClick={onAddOption} className="question-add-option-btn">
            + Dodaj opciju
        </button>
            )}
        </div>
      )}

      <div className="question-form-footer">
        <div className="points-input-group">
            <label htmlFor={`points-${questionIndex}`}>Broj bodova</label>
            <input
                id={`points-${questionIndex}`}
                type="text"
                className="points-input"
                value={displayValue}
                onChange={(e) => {
                    const value = e.target.value;
                    if (/^[0-9]*[,.]?[0-9]*$/.test(value)) {
                        setDisplayValue(value); 
                    }
                }}
                onBlur={() => {
                    let finalValue = parseFloat(displayValue.replace(',', '.'));
                    if (isNaN(finalValue) || finalValue < 0.5) {
                        finalValue = 0.5;
                    } else if (finalValue > 10) {
                        finalValue = 10;
                    }
                    handleChange('pointNum', finalValue);
                }}
            />
        </div>
      </div>
    </div>
  );
};

export default QuestionForm;
import React, { useState, useEffect, useCallback,useRef } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import quizService from '../../../../../services/quizService';
import QuizForm from '../components/quizForm/QuizForm';
import QuestionForm from '../components/questionForm/QuestionForm';
import Modal from '../../../../../components/Modal/Modal';
import './style.css';

const emptyQuestion = { 
  questionText: '', 
  type: 0, 
  pointNum: 1,
  correctTextAnswer: '',
  answerOptions: [{ text: '', isCorrect: false }, { text: '', isCorrect: false }] 
};

const QuizBuilderPage = () => {
  const { quizId } = useParams<{ quizId: string }>();
  const navigate = useNavigate();

  const [isConfirmModalOpen, setIsConfirmModalOpen] = useState(false);
  const [confirmModalMessage, setConfirmModalMessage] = useState('');

  const nextTempId = useRef(-1);
  const getNextTempId = () => {
        const tempId = nextTempId.current;
        nextTempId.current -= 1;
        return tempId;
    };

  const isEditMode = !!quizId;

  const [quizDetails, setQuizDetails] = useState({ name: '', description: '', difficulty: 0, timeLimit: 300, categoryIds: [] });
  const [questions, setQuestions] = useState(() => [
      {
          ...emptyQuestion,
          questionID: getNextTempId(),
          answerOptions: emptyQuestion.answerOptions.map(opt => ({
                ...opt,
                answerOptionID: getNextTempId() 
            }))
      }
  ]);
  const [loading, setLoading] = useState(isEditMode);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null); 

  const fetchData = useCallback(async () => {
    if (isEditMode && quizId) {
      try {
        const quizResponse  = await quizService.getQuizById(parseInt(quizId));
        const questionsResponse = await quizService.getQuestionsForQuiz(parseInt(quizId));
        const quizData = quizResponse.data;
        
        setQuizDetails({
          name: quizData.name,
          description: quizData.description,
          difficulty: quizData.difficulty === "Easy" ? 0 : quizData.difficulty === "Medium" ? 1 : 2,
          timeLimit: quizData.timeLimit,
          categoryIds: quizData.categories.map((c: any) => ({ 
            value: c.categoryID, 
            label: c.name 
          }))
        });

        const formattedQuestions = questionsResponse.data.map((q: any) => {
          let typeAsNumber = 0;
          if (q.type === 'MultipleChoice') typeAsNumber = 1;
          if (q.type === 'TrueFalse') typeAsNumber = 2;
          if (q.type === 'FillInTheBlank') typeAsNumber = 3;

          return { ...q, type: typeAsNumber };
        });

        setQuestions(formattedQuestions.length > 0 ? formattedQuestions : [emptyQuestion]);
      
      } catch (error) {
        console.error("Kviz nije pronađen", error);
        navigate('/dashboard/kvizovi');
      } finally {
        setLoading(false);
      }
    }
  }, [quizId, isEditMode, navigate]);

  useEffect(() => {
    if (isEditMode) {
      fetchData();
    }
  }, [fetchData, isEditMode]);

  const handleQuizDetailsChange = (field: string, value: any) => {
    setQuizDetails(prev => ({ ...prev, [field]: value }));
  };

  const handleQuestionChange = (index: number, updatedQuestionData: any) => {
    const newQuestions = [...questions];
    newQuestions[index] = updatedQuestionData;
    setQuestions(newQuestions);
  };

  const addQuestionForm = () => {
    setQuestions(prevQuestions => [
        ...prevQuestions,
        {
            ...emptyQuestion,
            questionID: getNextTempId(), 
            answerOptions: emptyQuestion.answerOptions.map(opt => ({
                ...opt,
                answerOptionID: getNextTempId() 
            }))
        }
    ]);
  };

  const removeQuestionForm = (indexToRemove: number) => {
    if (questions.length <= 1) return; // Uvek mora postojati bar jedno pitanje
    setQuestions(questions.filter((_, i) => i !== indexToRemove));
  };

  const handleConfirmArchive = async () => {
    setIsConfirmModalOpen(false);
    setIsSaving(true);

    const cleanedQuestions = questions.map(q => {
        if (q.type === 3) {
            return { ...q, answerOptions: [] };
        }
        return q;
    });

    const finalDto = {
      ...quizDetails,
      categoryIds: quizDetails.categoryIds.map((c: any) => c.value), 
      questions: cleanedQuestions
    };

    try {
        if (quizId) {
            await quizService.archiveAndCreateNew(parseInt(quizId), finalDto);
            alert('Stari kviz je arhiviran, a novi sa izmenama je uspešno kreiran!');
            navigate(`/dashboard/kvizovi/`);
        }
    } catch (archiveError: any) {
        setError(archiveError.response?.data?.message || 'Greška prilikom arhiviranja.');
    } finally {
        setIsSaving(false);
    }
  };

  const handleCancelArchive = () => {
      setIsConfirmModalOpen(false);
  };

  const handleSaveAll = async () => {
    setError(null);

    if (!quizDetails.name.trim()) {
      setError("Naziv kviza ne sme biti prazan.");
      return;
    }
    if (quizDetails.timeLimit < 15 || quizDetails.timeLimit > 600) {
      setError("Vreme mora biti između 15 i 600 sekundi.");
      return;
    }
    if (quizDetails.categoryIds.length === 0) {
      setError("Kviz mora imati bar jednu kategoriju.");
      return;
    }
    if (questions.length === 0 || (questions.length === 1 && !questions[0].questionText.trim())) {
        setError("Kviz mora imati bar jedno pitanje.");
        return;
    }

    for (let i = 0; i < questions.length; i++) {
      const q = questions[i];
      if (!q.questionText.trim()) {
        setError(`Pitanje #${i + 1} mora imati tekst.`);
        return;
      }

      if (q.type < 3) { 
        if (q.answerOptions.length < 2) {
          setError(`Pitanje #${i + 1} mora imati bar 2 ponuđena odgovora.`);
          return;
        }
        if (q.answerOptions.some(opt => !opt.text.trim())) {
          setError(`Svi ponuđeni odgovori za pitanje #${i + 1} moraju imati tekst.`);
          return;
        }
        if (q.type < 2 && !q.answerOptions.some(opt => opt.isCorrect)) {
          setError(`Pitanje #${i + 1} mora imati bar jedan tačan odgovor.`);
          return;
        }
      }

      if (q.type === 3 && !q.correctTextAnswer?.trim()) {
        setError(`Pitanje #${i + 1} mora imati definisan tačan tekstualni odgovor.`);
        return;
      }
    }

    const cleanedQuestions = questions.map(q => {
        if (q.type === 3) {
            return { ...q, answerOptions: [] };
        }
        return q;
    });

    const finalDto = {
      ...quizDetails,
      categoryIds: quizDetails.categoryIds.map((c: any) => c.value), 
      questions: cleanedQuestions
    };

    setIsSaving(true);
    
    try {
      if (isEditMode && quizId) {
        await quizService.updateQuiz(parseInt(quizId), finalDto);
        navigate(`/dashboard/kvizovi/`);
      } else {
        await quizService.createQuizWithQuestions(finalDto);
            navigate(`/dashboard/kvizovi/`);
      }
    } catch (error: any) {
      if (isEditMode && quizId && error.response && error.response.status === 409) {
            setConfirmModalMessage(error.response.data.message);
            setIsConfirmModalOpen(true);
        } else {
            setError(error.response?.data?.message || "Došlo je do greške pri čuvanju kviza.");
        }
    } finally {
        if (!isConfirmModalOpen) { 
             setIsSaving(false);
        }
    }
  };

  if (loading) {
    return <div>Učitavanje kviza...</div>
  }

  const handleAddAnswerOption = (questionIndex: number) => {
    setQuestions(prevQuestions => 
      prevQuestions.map((question, index) => {
        if (index !== questionIndex) {
          return question;
        }
        return {
          ...question, 
          answerOptions: [
            ...question.answerOptions,
            {
              answerOptionID: getNextTempId(),
              text: '',
              isCorrect: false
            }
          ]
        };
      })
    );
  };

  return (
    <>
    <form onSubmit={(e) => { e.preventDefault(); handleSaveAll(); }} className="form-page-container">
      
      <h2>{isEditMode ? `Uređivanje Kviza: ${quizDetails.name}` : 'Kreiraj Novi Kviz'}</h2>

      <QuizForm 
        quizData={quizDetails} 
        onDataChange={handleQuizDetailsChange} 
      />

      <hr className="section-divider" />

      <h3>Pitanja</h3>
      <div className="questions-container">
        {questions.map((q, index) => (
          <QuestionForm 
            key={q.questionID} 
            questionIndex={index}
            questionData={q}
            onDataChange={handleQuestionChange}
            onRemove={removeQuestionForm}
            onAddOption={() => handleAddAnswerOption(index)}
          />
        ))}
      </div>

      <button type="button" onClick={addQuestionForm} className="btn add-question-main-btn">
         + Dodaj Novo Pitanje
      </button>

      <hr className="section-divider" />

      {error && <div className="error-message-container">{error}</div>}

       <div className="form-footer">
        <div className="total-points-display">
            Ukupan broj bodova: <strong>
            {
              questions.reduce((total, question) => 
                total + (Number(question.pointNum) || 0), 0)
                .toLocaleString('sr-RS')
            }
          </strong>
        </div>
        <button type="submit" className="btn btn-primary" disabled={isSaving}>
          {isSaving ? 'Čuvanje...' : 'Sačuvaj Ceo Kviz'}
        </button>
      </div>
    </form>

<Modal isOpen={isConfirmModalOpen} onClose={handleCancelArchive}>
        <div className="confirmation-modal">
            <h3>Potvrda izmene</h3>
            {/* Prikazujemo poruku sa servera */}
            <p>{confirmModalMessage}</p> 
            <div className="modal-actions">
                <button type="button" onClick={handleCancelArchive} className="btn-secondary btn">
                    Poništi
                </button>
                <button type="button" onClick={handleConfirmArchive} className="btn">
                    OK
                </button>
            </div>
        </div>
      </Modal>
    </>
  );
};

export default QuizBuilderPage;
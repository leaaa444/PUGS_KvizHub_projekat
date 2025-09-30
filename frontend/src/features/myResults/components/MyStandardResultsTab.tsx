import React, { useState, useEffect } from 'react';
import resultService from '../../../services/resultService';
import ArchivedQuestion from './ArchivedQuestion';
import ProgressChart from './ProgressChart';

interface MyResult {
    resultId: number;
    quizName: string;
    dateCompleted: string;
    score: number;
    percentage: number;
    attemptNum: number; 
}

interface ArchivedResultDetails {
    quizId: number;
    quizName: string;
    description: string;      
    difficulty: string;      
    categories: string[]; 
    timeTaken: number;
    questions: any[];
}

const MyStandardResultsTab: React.FC = () => {
    const [results, setResults] = useState<MyResult[]>([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [modalLoading, setModalLoading] = useState(false);
    const [selectedQuizId, setSelectedQuizId] = useState<number | null>(null);
    const [selectedResultDetails, setSelectedResultDetails] = useState<ArchivedResultDetails | null>(null);
    const [loading, setLoading] = useState(true);
    

    useEffect(() => {
        resultService.getMyResults()
            .then(response => {
                setResults(response.data);
            })
            .catch(error => console.error("Greška pri preuzimanju rezultata:", error))
            .finally(() => setLoading(false));
    }, []);

    const handleOpenDetailsModal = async (resultId: number) => {
        setSelectedQuizId(resultId);
        setIsModalOpen(true);
        setModalLoading(true);
        try {
            const response = await resultService.getArchivedResultDetails(resultId);
            setSelectedResultDetails(response.data);
        } catch (error) {
            console.error("Greška pri preuzimanju rang liste za kviz:", error);
        } finally {
            setModalLoading(false);
        }
    };

    
     const handleCloseModal = () => {
        setIsModalOpen(false);
        setSelectedResultDetails(null); 
    };

    if (loading) {
        return <p>Učitavanje rezultata...</p>;
    }
    
    return (
        <>
            <div className="my-results-container">
                <h1>Moji Rezultati</h1>
                {results.length > 0 ? (
                    <table className="results-table">
                        <thead>
                            <tr>
                                <th>Naziv Kviza</th>
                                <th>Datum Rešavanja</th>
                                <th>Osvojeni Bodovi</th>
                                <th>Procenat</th>
                                <th>Pokušaj</th> 
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            {results.map(result => (
                                <tr key={result.resultId}>
                                    <td>{result.quizName}</td>
                                    <td>{new Date(result.dateCompleted).toLocaleDateString('sr-RS')}</td>
                                    <td>{result.score.toLocaleString('sr-RS')}</td>
                                    <td>{result.percentage.toFixed(1)}%</td>
                                    <td>{result.attemptNum}</td>
                                    <td>
                                        <button onClick={() => handleOpenDetailsModal(result.resultId)} className="btn-details">
                                            Pregled detalja
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                ) : (
                    <p>Još uvek niste rešili nijedan kviz.</p>
                )}
            </div>
            
            {isModalOpen && (
                <div className="custom-modal-overlay" onClick={handleCloseModal}>
                    <div className="custom-modal-content" onClick={(e) => e.stopPropagation()}>
                        <button onClick={handleCloseModal} className="custom-modal-close-btn">&times;</button>
                        
                        {modalLoading && <h2>Učitavanje detalja...</h2>}
                        {selectedResultDetails && (
                            <div className="archived-result-details">
                                <div className="modal-result-header">
                                    <h2>{selectedResultDetails.quizName}</h2>
                                    <p className="modal-quiz-description">{selectedResultDetails.description}</p>
                                    
                                    <div className="modal-quiz-meta">
                                        <span className={`modal-quiz-difficulty difficulty-${selectedResultDetails.difficulty.toLowerCase()}`}>
                                            {selectedResultDetails.difficulty}
                                        </span>
                                        <div className="modal-quiz-categories">
                                            {selectedResultDetails.categories?.map(cat => <span key={cat} className="modal-category-tag">{cat}</span>)}
                                        </div>
                                    </div>
                                    
                                    <p className="modal-time-taken">
                                        Vreme rešavanja: {Math.floor(selectedResultDetails.timeTaken / 60)} min {selectedResultDetails.timeTaken % 60} sec
                                    </p>
                                </div>
                                <hr />
                                <div className="questions-list">
                                <ProgressChart quizId={selectedResultDetails.quizId} />
                                    {selectedResultDetails.questions.map((q, index) => (
                                        <ArchivedQuestion key={q.questionId} question={q} index={index + 1} />
                                    ))}
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            )}
        </>
    );
};

export default MyStandardResultsTab;
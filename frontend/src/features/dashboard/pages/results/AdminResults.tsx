import React, { useState, useEffect, useCallback } from 'react';
import resultService from '../../../../services/resultService';
import quizService from '../../../../services/quizService';
import './AdminResultsPage.css'; 

interface Result {
    resultId: number;
    quizName: string;
    username: string;
    score: number;
    completionTime: number;
    dateOfCompletion: string;
}

interface PaginationInfo {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalCount: number;
}

interface QuizFilterItem {
    quizID: number;
    name: string;
}

const AdminResultsPage = () => {
    const [results, setResults] = useState<Result[]>([]);
    const [quizzes, setQuizzes] = useState<QuizFilterItem[]>([]);
    const [pagination, setPagination] = useState<PaginationInfo>({
        currentPage: 1,
        totalPages: 1,
        pageSize: 10,
        totalCount: 0,
    });
    const [filters, setFilters] = useState({ username: '', quizId: '' });
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchResults = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const response = await resultService.getAllAdminResults(
                pagination.currentPage,
                pagination.pageSize,
                filters.username,
                filters.quizId ? parseInt(filters.quizId, 10) : undefined
            );
            
            setResults(response.data.items);
            setPagination(prev => ({
                ...prev,
                totalPages: response.data.totalPages,
                totalCount: response.data.totalCount,
            }));
        } catch (err) {
            setError('Greška pri dohvatanju rezultata.');
        } finally {
            setLoading(false);
        }
    }, [pagination.currentPage, pagination.pageSize, filters]);

    useEffect(() => {
        fetchResults();
    }, [fetchResults]);

    useEffect(() => {
        const fetchQuizzesForFilter = async () => {
            try {
                const response = await quizService.getSoloQuizzes();
                setQuizzes(response.data);
            } catch (err) {
                console.error("Greška pri dohvatanju kvizova za filter", err);
            }
        };
        fetchQuizzesForFilter();
    }, []);

    const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setPagination(prev => ({ ...prev, currentPage: 1 }));
        setFilters(prev => ({ ...prev, [name]: value }));
    };

    const handlePageChange = (newPage: number) => {
        if (newPage > 0 && newPage <= pagination.totalPages) {
            setPagination(prev => ({ ...prev, currentPage: newPage }));
        }
    };

    const formatTime = (seconds: number) => {
        if (seconds < 60) return `${seconds}s`;
        const minutes = Math.floor(seconds / 60);
        const remainingSeconds = seconds % 60;
        return `${minutes}m ${remainingSeconds}s`;
    };

    return (
        <div className="admin-results-page">
            <h2>Pregled Svih Rezultata</h2>

            <div className="filters-container">
                <input
                    type="text"
                    name="username"
                    placeholder="Pretraži po korisniku..."
                    value={filters.username}
                    onChange={handleFilterChange}
                    className="filter-input"
                />
                <select
                    name="quizId"
                    value={filters.quizId}
                    onChange={handleFilterChange}
                    className="filter-select"
                >
                    <option value="">Svi Kvizovi</option>
                    {quizzes.map(quiz => (
                        <option key={quiz.quizID} value={quiz.quizID}>
                            {quiz.name}
                        </option>
                    ))}
                </select>
            </div>

            {loading && <p>Učitavanje rezultata...</p>}
            {error && <p className="error-message">{error}</p>}

            {!loading && !error && (
                <>
                    <div className="results-table-container">
                        <table>
                            <thead>
                                <tr>
                                    <th>Korisnik</th>
                                    <th>Kviz</th>
                                    <th>Poeni</th>
                                    <th>Vreme rešavanja</th>
                                    <th>Datum</th>
                                </tr>
                            </thead>
                            <tbody>
                                {results.length > 0 ? (
                                    results.map(result => (
                                        <tr key={result.resultId}>
                                            <td>{result.username}</td>
                                            <td>{result.quizName}</td>
                                            <td>{result.score.toLocaleString('sr-RS')}</td>
                                            <td>{formatTime(result.completionTime)}</td>
                                            <td>{new Date(result.dateOfCompletion).toLocaleDateString('sr-RS')}</td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan={5}>Nema rezultata za prikazane filtere.</td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>

                    <div className="pagination-container">
                        <button
                            onClick={() => handlePageChange(pagination.currentPage - 1)}
                            disabled={pagination.currentPage <= 1}
                        >
                            Prethodna
                        </button>
                        <span>
                            Stranica {pagination.currentPage} od {pagination.totalPages}
                        </span>
                        <button
                            onClick={() => handlePageChange(pagination.currentPage + 1)}
                            disabled={pagination.currentPage >= pagination.totalPages}
                        >
                            Sledeća
                        </button>
                    </div>
                </>
            )}
        </div>
    );
};

export default AdminResultsPage;
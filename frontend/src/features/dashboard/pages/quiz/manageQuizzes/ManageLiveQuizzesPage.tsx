import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Quiz } from '../types';
import QuizTable from '../components/quizTable/QuizTable';
import quizService from '../../../../../services/quizService';
import { connection, startConnection, createRoom } from '../../../../../services/signalrService'; 
import './style.css';

const ManageLiveQuizzesPage = () => {
    const [quizzes, setQuizzes] = useState<Quiz[]>([]);
    const navigate = useNavigate();

    useEffect(() => {
        quizService.getLiveQuizzes().then((response) => {
            setQuizzes(response.data);
        });
        startConnection(); 
    }, []);

    useEffect(() => {
        const handleRoomCreated = (roomCode: string) => {
            console.log(`Soba je kreirana! Kod: ${roomCode}`);
            navigate(`/live-arena/lobby/${roomCode}`);
        };
        connection.on("RoomCreated", handleRoomCreated);
        
        return () => { connection.off("RoomCreated", handleRoomCreated); };
    }, [navigate]);

    const handleStartArena = (quizId: number) => {
        console.log(`Pokrećem arenu za kviz sa ID-jem: ${quizId}`);
        createRoom(quizId);
    };

    const handleEdit = (id: number) => {
        navigate(`/dashboard/kvizovi/edit/${id}`);
    };

    const handleDelete = async (id: number) => {
        if (window.confirm('Da li ste sigurni?')) {
            await quizService.deleteQuiz(id);
            setQuizzes(quizzes.filter(q => q.quizID !== id));
        }
    };

    return (
        <div className="manage-page-container">
            <div className="manage-header">
                <h2>Upravljanje Live Kvizovima</h2>
                <button onClick={() => navigate('/dashboard/live-kvizovi/novi')} className="btn btn-primary">
                    Dodaj Novi Live Kviz
                </button>
            </div>

            <QuizTable
                quizzes={quizzes}
                mode="live"
                onEdit={handleEdit}
                onDelete={handleDelete}
                onStartArena={handleStartArena}
            />
        </div>
    );
};

export default ManageLiveQuizzesPage;
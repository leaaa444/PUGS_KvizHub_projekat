import React, { useState, useEffect } from 'react';
import MyLiveArenasTab from './components/MyLiveArenasTab';
import MyStandardResultsTab from './components/MyStandardResultsTab';
import './MyResultsPage.css';

type Tab = 'standard' | 'live';

const MyResultsPage: React.FC = () => {    
    const [activeTab, setActiveTab] = useState<Tab>('standard');

    return (
        <div className="my-results-container">
            <h1>Moji Rezultati</h1>

            <div className="tabs-container">
                <button 
                    className={`tab-button ${activeTab === 'standard' ? 'active' : ''}`}
                    onClick={() => setActiveTab('standard')}
                >
                    Standardni Kvizovi
                </button>
                <button 
                    className={`tab-button ${activeTab === 'live' ? 'active' : ''}`}
                    onClick={() => setActiveTab('live')}
                >
                    Live Arene
                </button>
            </div>

            <div className="tab-content">
                {activeTab === 'standard' ? <MyStandardResultsTab /> : <MyLiveArenasTab />}
            </div>
        </div>
    );
}

export default MyResultsPage;
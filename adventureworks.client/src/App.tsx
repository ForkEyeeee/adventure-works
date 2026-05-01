import { useEffect, useState } from 'react';
import './App.css';

interface Forecast {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}

interface BusinessEntity {
    businessEntityID: number;
    rowguid: string;
    modifiedDate: string;
}

function App() {
    const [forecasts, setForecasts] = useState<Forecast[]>();
    const [businessEntities, setBusinessEntities] = useState<BusinessEntity[]>([]);
    const [newEntity, setNewEntity] = useState({
        rowguid: '',
        modifiedDate: ''
    });
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        populateWeatherData();
        populateBusinessEntities();
    }, []);

    const handleAddEntity = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        try {
            const response = await fetch('weatherforecast/businessentity', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    rowguid: newEntity.rowguid || crypto.randomUUID(),
                    modifiedDate: newEntity.modifiedDate || new Date().toISOString()
                })
            });
            if (response.ok) {
                setNewEntity({
                    rowguid: '',
                    modifiedDate: ''
                });
                populateBusinessEntities();
            } else {
                alert('Failed to add business entity');
            }
        } catch (error) {
            console.error('Error adding entity:', error);
            alert('Error adding business entity');
        } finally {
            setLoading(false);
        }
    };

    const handleDeleteEntity = async (id: number) => {
        if (!window.confirm('Are you sure you want to delete this entity?')) {
            return;
        }
        setLoading(true);
        try {
            const response = await fetch(`weatherforecast/businessentity/${id}`, {
                method: 'DELETE'
            });
            if (response.ok) {
                populateBusinessEntities();
            } else {
                alert('Failed to delete business entity');
            }
        } catch (error) {
            console.error('Error deleting entity:', error);
            alert('Error deleting business entity');
        } finally {
            setLoading(false);
        }
    };

    const weatherContents = forecasts === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <table className="table table-striped" aria-labelledby="tableLabel">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Temp. (C)</th>
                    <th>Temp. (F)</th>
                    <th>Summary</th>
                </tr>
            </thead>
            <tbody>
                {forecasts.map(forecast =>
                    <tr key={forecast.date}>
                        <td>{forecast.date}</td>
                        <td>{forecast.temperatureC}</td>
                        <td>{forecast.temperatureF}</td>
                        <td>{forecast.summary}</td>
                    </tr>
                )}
            </tbody>
        </table>;

    return (
        <div>
            <h1 id="tableLabel">Weather forecast</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {weatherContents}

            <hr />

            <h2>Business Entities Management</h2>
            
            <div className="card" style={{ marginBottom: '20px', padding: '15px' }}>
                <h3>Add New Business Entity</h3>
                <form onSubmit={handleAddEntity}>
                    <div className="form-group">
                        <label htmlFor="rowguid">Row GUID (optional):</label>
                        <input
                            type="text"
                            id="rowguid"
                            className="form-control"
                            placeholder="Leave blank for auto-generated UUID"
                            value={newEntity.rowguid}
                            onChange={(e) => setNewEntity({ ...newEntity, rowguid: e.target.value })}
                        />
                    </div>
                    <button 
                        type="submit" 
                        className="btn btn-primary"
                        disabled={loading}
                    >
                        {loading ? 'Adding...' : 'Add Entity'}
                    </button>
                </form>
            </div>

            <h3>Business Entities List</h3>
            {businessEntities.length === 0 ? (
                <p><em>No business entities found.</em></p>
            ) : (
                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>Business Entity ID</th>
                            <th>Row GUID</th>
                            <th>Modified Date</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {businessEntities.map(entity =>
                            <tr key={entity.businessEntityID}>
                                <td>{entity.businessEntityID}</td>
                                <td>{entity.rowguid}</td>
                                <td>{new Date(entity.modifiedDate).toLocaleString()}</td>
                                <td>
                                    <button
                                        className="btn btn-danger btn-sm"
                                        onClick={() => handleDeleteEntity(entity.businessEntityID)}
                                        disabled={loading}
                                    >
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            )}
        </div>
    );

    async function populateWeatherData() {
        const response = await fetch('weatherforecast');
        if (response.ok) {
            const data = await response.json();
            setForecasts(data);
        }
    }

    async function populateBusinessEntities() {
        try {
            const response = await fetch('weatherforecast/businessentity');
            if (response.ok) {
                const data = await response.json();
                setBusinessEntities(data);
            }
        } catch (error) {
            console.error('Error fetching business entities:', error);
        }
    }
}

export default App;
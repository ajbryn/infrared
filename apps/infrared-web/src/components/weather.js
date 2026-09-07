import { useEffect, useState } from 'react';
import { api } from '../api/client';

export default function Weather() {
    const [weather, setWeather] = useState([]);

    const getWeather = async () => {
        try {
            const data = await api.get("/weatherforecast");
            setWeather(data);
        } catch (error) {
            console.error("Error fetching weather data:", error);
        }
    };

    useEffect(() => {
        getWeather();
    },[]);

    return (
        <>
            {
                weather.map((item, index) => (
                    <div key={index}>
                        <span>Date: {item.date}</span>
                        <span>TemperatureF: {item.temperatureF}</span>
                        <span>Summary: {item.summary}</span>
                    </div>
                ))
            }
        </>
    );
};
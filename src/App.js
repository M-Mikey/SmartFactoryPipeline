import { useState, useEffect } from "react";
import axios from "axios";
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from "recharts";

const tc = t => t > 80 ? "#E24B4A" : t > 70 ? "#EF9F27" : "#639922";
const fmt = ts => new Date(ts).toLocaleTimeString([], {hour:'2-digit',minute:'2-digit',second:'2-digit'});

const MetricCard = ({ label, value, footer, accent, valueColor }) => (
  <div style={{background:"#fff", border:"1px solid #e2e8f0", borderRadius:"8px", padding:"14px 16px", position:"relative", overflow:"hidden"}}>
    <div style={{position:"absolute", top:0, left:0, width:"3px", height:"100%", background: accent, borderRadius:"4px 0 0 4px"}} />
    <div style={{fontSize:"11px", color:"#94a3b8", textTransform:"uppercase", letterSpacing:"0.05em", marginBottom:"8px"}}>{label}</div>
    <div style={{fontSize:"26px", fontWeight:"500", color: valueColor || "#0f172a"}}>{value}</div>
    <div style={{fontSize:"11px", color:"#94a3b8", marginTop:"6px"}}>{footer}</div>
  </div>
);

function App() {
  const [readings, setReadings] = useState([]);
  const [lastUpdate, setLastUpdate] = useState("—");

  useEffect(() => {
    const load = () => axios.get("http://localhost:5004/api/machine")
      .then(r => { setReadings([...r.data].reverse()); setLastUpdate(fmt(new Date())); })
      .catch(console.error);
    load();
    const id = setInterval(load, 3000);
    return () => clearInterval(id);
  }, []);

  const latest = readings[readings.length - 1];
  const temps = readings.map(r => r.temperature);
  const avg = temps.length ? Math.round(temps.reduce((a,b)=>a+b,0)/temps.length) : "—";
  const max = temps.length ? Math.max(...temps) : "—";
  const alerts = temps.filter(t => t > 80).length;

  return (
    <div style={{background:"#f8fafc", minHeight:"100vh", fontFamily:"Arial"}}>
      
      {/* Top bar */}
      <div style={{background:"#fff", borderBottom:"1px solid #e2e8f0", padding:"0 1.5rem", height:"52px", display:"flex", alignItems:"center", justifyContent:"space-between"}}>
        <div style={{display:"flex", alignItems:"center", gap:"12px"}}>
          <div style={{width:"28px", height:"28px", background:"#185FA5", borderRadius:"6px", display:"flex", alignItems:"center", justifyContent:"center", color:"white", fontSize:"14px", fontWeight:"bold"}}>SF</div>
          <div>
            <div style={{fontSize:"14px", fontWeight:"500", color:"#0f172a"}}>Smart Factory — Machine Monitor</div>
            <div style={{fontSize:"11px", color:"#94a3b8"}}>Machine1 · Oracle 19c · ASP.NET Core API</div>
          </div>
        </div>
        <div style={{display:"flex", alignItems:"center", gap:"8px"}}>
          <span style={{fontSize:"11px", color:"#94a3b8"}}>Updated {lastUpdate}</span>
          <div style={{width:"7px", height:"7px", borderRadius:"50%", background:"#639922"}} />
          <span style={{fontSize:"12px", color:"#3B6D11", fontWeight:"500"}}>Live</span>
        </div>
      </div>

      <div style={{padding:"1.25rem 1.5rem"}}>
        
        {/* Metric cards */}
        <div style={{display:"grid", gridTemplateColumns:"repeat(4,1fr)", gap:"10px", marginBottom:"1.25rem"}}>
          <MetricCard label="Current temperature" value={latest ? latest.temperature+"°C" : "—"} footer={latest ? "as of "+fmt(latest.timestamp) : "waiting..."} accent="#185FA5" valueColor={latest ? tc(latest.temperature) : "#0f172a"} />
          <MetricCard label="Average (last 20)" value={avg+"°C"} footer="across last 20 readings" accent="#3B6D11" />
          <MetricCard label="Peak temperature" value={max+"°C"} footer="last 20 readings" accent="#854F0B" valueColor={max !== "—" ? tc(max) : "#0f172a"} />
          <MetricCard label="High temp events" value={alerts} footer="readings above 80°C" accent="#A32D2D" valueColor={alerts > 0 ? "#E24B4A" : "#639922"} />
        </div>

        <div style={{display:"grid", gridTemplateColumns:"1fr 320px", gap:"10px"}}>
          
          {/* Chart */}
          <div style={{background:"#fff", border:"1px solid #e2e8f0", borderRadius:"8px", padding:"14px 16px"}}>
            <div style={{display:"flex", justifyContent:"space-between", alignItems:"center", marginBottom:"12px"}}>
              <span style={{fontSize:"13px", fontWeight:"500", color:"#0f172a"}}>Temperature trend</span>
              <div style={{display:"flex", gap:"14px"}}>
                {[["#E24B4A","Hot >80°C"],["#EF9F27","Warm 70–80°C"],["#639922","Normal <70°C"]].map(([c,l])=>(
                  <div key={l} style={{display:"flex", alignItems:"center", gap:"5px", fontSize:"11px", color:"#64748b"}}>
                    <div style={{width:"8px", height:"8px", borderRadius:"50%", background:c}} />{l}
                  </div>
                ))}
              </div>
            </div>
            <ResponsiveContainer width="100%" height={200}>
              <LineChart data={readings}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" />
                <XAxis dataKey="id" tick={{fontSize:10, fill:"#94a3b8"}} />
                <YAxis domain={[50,100]} tick={{fontSize:10, fill:"#94a3b8"}} tickFormatter={v=>v+"°"} />
                <Tooltip contentStyle={{background:"#fff", border:"1px solid #e2e8f0", fontSize:"12px"}} formatter={v=>[v+"°C","Temp"]} />
                <Line type="monotone" dataKey="temperature" stroke="#185FA5" strokeWidth={1.5} dot={p=><circle key={p.cx} cx={p.cx} cy={p.cy} r={4} fill={tc(p.value)} stroke="#185FA5" strokeWidth={1.5}/>} />
              </LineChart>
            </ResponsiveContainer>
          </div>

          {/* Recent readings */}
          <div style={{background:"#fff", border:"1px solid #e2e8f0", borderRadius:"8px", padding:"14px 16px"}}>
            <div style={{display:"flex", justifyContent:"space-between", alignItems:"center", marginBottom:"12px"}}>
              <span style={{fontSize:"13px", fontWeight:"500", color:"#0f172a"}}>Recent readings</span>
              <span style={{fontSize:"11px", color:"#94a3b8"}}>latest 10</span>
            </div>
            {[...readings].reverse().slice(0,10).map(r => (
              <div key={r.id} style={{display:"flex", alignItems:"center", gap:"8px", padding:"6px 0", borderBottom:"1px solid #f1f5f9"}}>
                <span style={{fontSize:"11px", fontFamily:"monospace", color:"#94a3b8", width:"36px"}}>#{r.id}</span>
                <div style={{flex:1, height:"4px", background:"#f1f5f9", borderRadius:"2px"}}>
                  <div style={{width:`${Math.round(((r.temperature-50)/50)*100)}%`, height:"100%", background:tc(r.temperature), borderRadius:"2px"}} />
                </div>
                <span style={{fontSize:"12px", fontWeight:"500", color:tc(r.temperature), width:"40px", textAlign:"right"}}>{r.temperature}°C</span>
                <span style={{fontSize:"10px", color:"#94a3b8", width:"60px", textAlign:"right"}}>{fmt(r.timestamp)}</span>
              </div>
            ))}
          </div>

        </div>
      </div>
    </div>
  );
}

export default App;
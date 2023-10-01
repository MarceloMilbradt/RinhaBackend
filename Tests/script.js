import http from 'k6/http';
import { sleep, check } from 'k6';
import { SharedArray } from 'k6/data';
import { Counter } from 'k6/metrics';

// Load data from tsv
const pessoasPayloads = new SharedArray("pessoasPayloads", () => open('pessoas-payloads.tsv').split('\n'));
const termosBusca = new SharedArray("termosBusca", () => open('termos-busca.tsv').split('\n'));

let locationCounter = new Counter('locationCounter');

export let options = {
  stages: [
    { duration: '10s', target: 2 }, // warm up
    { duration: '15s', target: 5 }, // are you ready?
    { duration: '3m', target: 600 }, // lezzz go!!!

    // Add other stages for the other scenarios as needed
  ],
  thresholds: {
    http_req_duration: ['p(95)<500'], // Sample threshold, adjust as needed
  },
};

export default function () {
  let payload = pessoasPayloads[Math.floor(Math.random() * pessoasPayloads.length)];
  let term = termosBusca[Math.floor(Math.random() * termosBusca.length)];

  // Criação E Talvez Consulta de Pessoas
  let criaRes = http.post('http://localhost:5224/pessoas', payload, {
    headers: {
      'User-Agent': 'Agente do Caos - 2023',
      'Content-Type': 'application/json',
    },
  });

  check(criaRes, {
    'criação status is 201, 422, or 400': r => [201, 422, 400].includes(r.status),
  });

  if (criaRes.status === 201) {
    locationCounter.add(1);
    let location = criaRes.headers.Location;
    let consultaRes = http.get(location);
  }

  sleep(Math.random() * (0.03 - 0.001) + 0.001); // pause

  // Busca Válida de Pessoas
  let buscaValidaRes = http.get(`http://localhost:5224/pessoas?t=${encodeURIComponent(term)}`);
  if (buscaValidaRes.status >= 300) {
    console.log(term);
  }
  check(buscaValidaRes, {
    'busca válida status is 2xx': r => r.status >= 200 && r.status < 300,
  });

  // Busca Inválida de Pessoas
  let buscaInvalidaRes = http.get('http://localhost:5224/pessoas');
  check(buscaInvalidaRes, {
    'busca inválida status is 400': r => r.status === 400,
  });
}

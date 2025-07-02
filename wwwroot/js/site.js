document.addEventListener('DOMContentLoaded', function() {
    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                window.scrollTo({
                    top: target.offsetTop - 80,
                    behavior: 'smooth'
                });
            }
        });
    });

    // Navbar scroll shadow
    const navbar = document.querySelector('.navbar');
    if (navbar) {
        window.addEventListener('scroll', function() {
            if (window.scrollY > 10) {
                navbar.classList.add('shadow-lg');
                navbar.style.padding = '0.5rem 1rem';
            } else {
                navbar.classList.remove('shadow-lg');
                navbar.style.padding = '1rem';
            }
        });
    }
/*
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(tooltipTriggerEl => {
        new bootstrap.Tooltip(tooltipTriggerEl, {
            placement: 'top',
            trigger: 'hover focus',
            animation: true,
            delay: { show: 50, hide: 50 },
            offset: [0, 8], // Adjusted offset for better positioning
            fallbackPlacements: ['bottom', 'right', 'left'],
            container: 'body',
            popperConfig: function(defaultBsPopperConfig) {
                return {
                    ...defaultBsPopperConfig,
                    modifiers: [
                        {
                            name: 'preventOverflow',
                            options: { boundary: document.body }
                        },
                        {
                            name: 'offset',
                            options: { offset: [0, 8] }
                        },
                        {
                            name: 'computeStyles',
                            options: { gpuAcceleration: false }
                        }
                    ]
                };
            }
        });
    });
    */
    // Initialize popovers
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.forEach(popoverTriggerEl => {
        new bootstrap.Popover(popoverTriggerEl);
    });

    // Animate counters
    const animateCounters = () => {
        const counters = document.querySelectorAll('.counter');
        const speed = 200;

        counters.forEach(counter => {
            const updateCount = () => {
                const target = +counter.getAttribute('data-target');
                const count = +counter.innerText.replace(/,/g, '');
                const increment = target / speed;

                if (count < target) {
                    counter.innerText = Math.ceil(count + increment);
                    setTimeout(updateCount, 1);
                } else {
                    counter.innerText = target.toLocaleString();
                }
            };
            updateCount();
        });
    };

    // Observe elements for animations
    const observeElements = () => {
        const elementsToAnimate = document.querySelectorAll('.animate-on-scroll');
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animated');
                    if (entry.target.classList.contains('counter-container')) {
                        animateCounters();
                    }
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1 });
        
        elementsToAnimate.forEach(element => observer.observe(element));
    };
    observeElements();

    // Form validation
    const forms = document.querySelectorAll('.needs-validation');
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });

    // Progress bar animation
    const progressBars = document.querySelectorAll('.progress-bar-animated');
    progressBars.forEach(bar => {
        const value = bar.getAttribute('aria-valuenow');
        bar.style.width = '0%';
        setTimeout(() => {
            bar.style.width = value + '%';
        }, 100);
    });

    // Workout card hover effects
    const workoutCards = document.querySelectorAll('.workout-plan-card');
    workoutCards.forEach(card => {
        card.addEventListener('mouseenter', () => {
            card.classList.add('shadow-lg');
            card.style.transform = 'translateY(-5px)';
        });
        card.addEventListener('mouseleave', () => {
            card.classList.remove('shadow-lg');
            card.style.transform = 'translateY(0)';
        });
    });

    // Dark mode toggle
    const darkModeToggle = document.getElementById('darkModeToggle');
    if (darkModeToggle) {
        darkModeToggle.addEventListener('click', () => {
            document.body.classList.toggle('dark-mode');
            const isDarkMode = document.body.classList.contains('dark-mode');
            localStorage.setItem('darkMode', isDarkMode);
            const icon = darkModeToggle.querySelector('i');
            if (isDarkMode) {
                icon.classList.remove('fa-moon');
                icon.classList.add('fa-sun');
            } else {
                icon.classList.remove('fa-sun');
                icon.classList.add('fa-moon');
            }
        });
        const savedDarkMode = localStorage.getItem('darkMode');
        if (savedDarkMode === 'true') {
            document.body.classList.add('dark-mode');
            const icon = darkModeToggle.querySelector('i');
            icon.classList.remove('fa-moon');
            icon.classList.add('fa-sun');
        }
    }

    // Fix input group alignment
    const fixInputGroupAlignment = () => {
        const inputGroups = document.querySelectorAll('.input-group');
        inputGroups.forEach(group => {
            const controls = group.querySelectorAll('.form-control, .form-select');
            const buttons = group.querySelectorAll('.btn');
            const spans = group.querySelectorAll('.input-group-text');
            
            const height = '45px';
            [...controls, ...buttons, ...spans].forEach(el => {
                el.style.height = height;
                el.style.minHeight = height;
                el.style.maxHeight = height;
                el.style.lineHeight = '1.5';
                el.style.boxSizing = 'border-box';
                
                if (el.classList.contains('btn') || el.classList.contains('input-group-text')) {
                    el.style.display = 'inline-flex';
                    el.style.alignItems = 'center';
                    el.style.justifyContent = 'center';
                    el.style.paddingTop = '0';
                    el.style.paddingBottom = '0';
                    el.style.verticalAlign = 'middle';
                }
            });
        });
    };

    // Run alignment fix on load and DOM changes
    fixInputGroupAlignment();
    const observer = new MutationObserver(fixInputGroupAlignment);
    observer.observe(document.body, { childList: true, subtree: true });
});